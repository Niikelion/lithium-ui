using UnityEngine.UIElements;

namespace UI.Li.Common.UIElements
{
    public sealed class DropdownField: PopupField<(int id, string name)>
    {
        public DropdownField()
        {
            formatListItemCallback = FormatItem;
            formatSelectedValueCallback = FormatItem;
        }

        private static string FormatItem((int id, string name) item) => item.name;
    }
}