using System.Linq;
using Xamarin.Forms;

namespace QuickTest
{
    public static class ElementRendering
    {
        public static string Render(this Element element)
        {
            if (!(element as VisualElement)?.IsVisible ?? false)
                return "";

            var result = "· ";

            result += (element as NavigationPage)?.CurrentPage.Render();
            if (element is ContentPage)
                result += (element as ContentPage).Title + " " + string.Join(" ", (element as Page).ToolbarItems.Select(t => $"[{t.Text}]"));
            result += (element as ContentPage)?.Content.Render();

            result += (element as ContentView)?.Content.Render();
            result += (element as ScrollView)?.Content.Render();
            result += string.Join("", (element as Layout<View>)?.Children.Select(c => c.Render()) ?? new[] { "" });
            result += (element as ListView)?.Render();

            result += (element as Label)?.Text;
            result += (element as Button)?.Text;
            result += (element as Entry)?.Text;
            result += (element as Editor)?.Text;
            result += (element as SearchBar)?.Text;

            var automationId = (element as VisualElement)?.AutomationId;
            if (automationId != null)
                result += $" ({automationId})";

            result = "\n" + result.Replace("\n", "\n  ");

            return result;
        }

        public static string Render(this ListView listView)
        {
            var result = "";
            if (listView.ItemsSource == null)
                return result;

            foreach (var item in listView.ItemsSource) {
                var content = listView.ItemTemplate.CreateContent();
                (content as Cell).BindingContext = item;
                result += $"- {(content as TextCell)?.Text + (content as ViewCell)?.View.Render().Trim()}\n";
            }

            return result;
        }
    }
}
