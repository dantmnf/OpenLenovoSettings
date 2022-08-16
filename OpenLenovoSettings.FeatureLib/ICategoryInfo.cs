using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{
    public interface ICategoryInfo
    {
        string Title { get; }
        int Order { get; }
        string? Icon { get; }
    }

    public record class DefaultCategoryInfo(string Title, int Order, string Icon) : ICategoryInfo;
}
