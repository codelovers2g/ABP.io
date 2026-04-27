using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagement.Localization;
using Volo.Abp.Application.Services;

namespace InventoryManagement;

/* Inherit your application services from this class.
 */
public abstract class InventoryManagementAppService : ApplicationService
{
    protected InventoryManagementAppService()
    {
        LocalizationResource = typeof(InventoryManagementResource);
    }
}
