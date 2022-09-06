using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using UrvinFinance.CommunityService.Application.Contracts.Services.CashTags;
using UrvinFinance.CommunityService.Communities;
using UrvinFinance.CommunityService.Domain.Users;
using UrvinFinance.CommunityService.UserProfiles;
using UrvinFinance.CommunityService.Users;
using UrvinFinance.Shared.Global.Constants;
using UrvinFinance.Shared.Global.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace UrvinFinance.CommunityService.Comments;

public class CommentAppService : CommunityServiceAppServiceBase, ICommentAppService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICommunityPostRepository _communityPostRepository;
    private readonly IUserProfileAppService _userProfileAppService;
    private readonly ILogger<CommentAppService> _logger;
    private IUserRepository _userRepository { get; }
    public IDistributedEventBus DistributedEventBus { get; }
    private CommentManager _commentManager { get; }

    public CommentAppService(ICommentRepository commentRepository,
        IUserRepository userRepository,
        IDistributedEventBus distributedEventBus,
        CommentManager commentManager,
        ICommunityPostRepository communityPostRepository,
        IUserProfileAppService userProfileAppService,
        ILogger<CommentAppService> logger)
    {
        _commentRepository = commentRepository;
        _communityPostRepository = communityPostRepository;
        _userProfileAppService = userProfileAppService;
        _logger = logger;
        _userRepository = userRepository;
        DistributedEventBus = distributedEventBus;
        _commentManager = commentManager;
    }

    [Authorize]
    public async Task<CommentDto> CreateAsync(string entityType, string entityId, CreateCommentInput input)
    {
        var user = await _userRepository.GetAsync(CurrentUser.Id.Value);

        if (input.RepliedCommentId.HasValue)
        {
            await _commentRepository.GetAsync(input.RepliedCommentId.Value);
        }

        var comment = await _commentRepository.InsertAsync(
            await _commentManager.CreateAsync(
                user,
                entityType,
                entityId,
                input.Text,
                input.RepliedCommentId
            )
        );

        await UnitOfWorkManager.Current.SaveChangesAsync();

        BackgroundJob.Enqueue<ICashTagMentionsAppService>(u => u.CreateContentCashTagMentionsAsync(comment.Id, CashTagMentionEntityType.Comment, CurrentUser.Id.Value, input.Text));
       
        await DistributedEventBus.PublishAsync(new CreatedCommentEvent
        {
            Id = comment.Id
        });

        return ObjectMapper.Map<Comment, CommentDto>(comment);
    }

    public async Task<ListResultDto<CommentWithDetailsDto>> GetListAsync(string entityType, string entityId)
    {
        var commentsWithAuthor = await _commentRepository
            .GetListWithAuthorsAsync(entityType, entityId);

        return new ListResultDto<CommentWithDetailsDto>(
            await ConvertCommentsToNestedStructure(commentsWithAuthor)
        );
    }

    public async Task<ListResultDto<CommentWithDetailsDto>> GetRepliesAsync(string commentId, GetListCommentInput input)
    {
        var repliesWithAuthor = await _commentRepository.GetListAsync(
                input.Filter,
                input.EntityType,
                Guid.Parse(commentId),
                input.AuthorUsername,
                input.CreationStartDate,
                input.CreationEndDate
            );

        var replies = repliesWithAuthor.Select(c => ObjectMapper.Map<Comment, CommentWithDetailsDto>(c.Comment)).ToList();

        foreach (var reply in replies)
        {
            reply.Author = await GetAuthorAsDtoFromCommentList(repliesWithAuthor, reply.Id);
        }

        return new ListResultDto<CommentWithDetailsDto>(replies);
    }

    public async Task<ListResultDto<CommentsFeedDto>> GetCommentsFeed(CommentsFeedInput input)
    {
        var commentsFeed = new List<CommentsFeedDto>();
        var rawComments = await _commentRepository.GetCommentsListForCommentsFeed(input
        );
        var comments = await ConvertCommentsToNestedStructure(
            rawComments
            );

        foreach (var comment in comments)
        {
            Guid id = new Guid(comment.EntityId);
            var post = await _communityPostRepository.GetByIdAsync(id: id);
            var commentFeedItem = ObjectMapper.Map<CommentWithDetailsDto, CommentsFeedDto>(comment);
            commentFeedItem.PostSlug = post.Slug;
            commentFeedItem.PostTitle = post.Title;
            commentFeedItem.PostCreationDate = post.CreationTime;
            commentFeedItem.PostCommunityName = post.Author.UserName;
            commentsFeed.Add(commentFeedItem);
        }

        return new ListResultDto<CommentsFeedDto>(commentsFeed);
    }

    [Authorize]
    public async Task<CommentDto> UpdateAsync(Guid id, UpdateCommentInput input)
    {
        var comment = await _commentRepository.GetAsync(id);

        if (comment.CreatorId != CurrentUser.GetId())
            throw new AbpAuthorizationException();

        comment.SetText(input.Text);
        comment.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        var updateComment = await _commentRepository.UpdateAsync(comment);

        BackgroundJob.Enqueue<ICashTagMentionsAppService>(u => u.UpdateContentCashTagMentionsAsync(comment.Id, CashTagMentionEntityType.Comment, CurrentUser.Id.Value, input.Text));

        return ObjectMapper.Map<Comment, CommentDto>(updateComment);
    }

    private async Task<List<CommentWithDetailsDto>> ConvertCommentsToNestedStructure(List<CommentWithAuthorQueryResultItem> comments)
    {
        var parentComments = comments
            .Where(c => c.Comment.RepliedCommentId == null)
            .Select(c => ObjectMapper.Map<Comment, CommentWithDetailsDto>(c.Comment))
            .ToList();

        foreach (var parentComment in parentComments)
        {
            parentComment.Author = await GetAuthorAsDtoFromCommentList(comments, parentComment.Id);

            parentComment.Replies = comments
                .Where(c => c.Comment.RepliedCommentId == parentComment.Id)
                .Select(c => ObjectMapper.Map<Comment, CommentDto>(c.Comment))
                .ToList();

            foreach (var reply in parentComment.Replies)
            {
                reply.Author = await GetAuthorAsDtoFromCommentList(comments, reply.Id);
            }
        }

        return parentComments;
    }

    private async Task<UserDto> GetAuthorAsDtoFromCommentList(List<CommentWithAuthorQueryResultItem> comments, Guid commentId)
    {
        var userDto = ObjectMapper.Map<User, UserDto>(comments.Single(c => c.Comment.Id == commentId).Author);

        userDto.ProfileImageDataUrl = await GetAuthorProfileImageDataUrl(userDto.Id);

        return userDto;
    }

    private async Task<string> GetAuthorProfileImageDataUrl(Guid id)
    {
        try
        {
            return await _userProfileAppService.GetUserProfilePictureDataUrl(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"_userProfileAppService.GetUserProfilePictureDataUrl({id}) failed");
            return string.Empty;
        }
    }

    public async Task<Boolean> DeleteAsync(Guid id)
    {
        var comment = await _commentRepository.GetAsync(id);


        if (comment.CreatorId == CurrentUser.Id || CurrentUser.IsInRole(GobalRoleConstants.Admin))
        {
            await _commentRepository.DeleteAsync(id);
            return true;
        }
        else
        {
            throw new AbpAuthorizationException();
        }
    }
}
