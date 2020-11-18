using System;
using Xamarin.Forms;

namespace DemoApp
{
    public class MenuPage : ContentPage
    {
        public MenuPage()
        {
            Title = "Menu";

            Content = new StackLayout {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    new DemoLabel("Menu"),
                    CreateMenuButton("Navigation", () => new NavigationDemoPage()),
                    CreateMenuButton("Elements", () => new ElementDemoPage()),
                    CreateListViewButton(ListViewCachingStrategy.RetainElement),
                    CreateListViewButton(ListViewCachingStrategy.RecycleElement),
                    CreateListViewButton(ListViewCachingStrategy.RecycleElementAndDataTemplate),
                    CreateMenuButton("Binding", () => new BindingDemoPage()),
                    CreateMenuButton("Popups", () => new PopupDemoPage()),
                    CreateMenuButton("TabbedPage", () => new TabbedPageDemoPage()),
                    CreateMenuButton("CarouselPage", () => new CarouselDemoPage()),
                    CreateMenuButton("TitleViewPage", () => new TitleViewPage()),
                    CreateMenuButton("TabbedTitleViewPage", () => new TabbedTitleViewPage()),
                    CreateMenuButton("Empty ContentPage", () => new ContentPage() { Title = "Page with no content", Content = null })
                },
            };
        }

        DemoButton CreateListViewButton(ListViewCachingStrategy cachingStrategy)
        {
            return CreateMenuButton($"ListViews ({cachingStrategy})", () => new ListViewDemoPage(cachingStrategy));
        }

        DemoButton CreateMenuButton(string title, Func<Page> pageCreator)
        {
            return new DemoButton(title) {
                Command = new Command(o => {
                    var mainPage = (Application.Current.MainPage as MasterDetailPage);
                    mainPage.Detail = new NavigationPage(pageCreator.Invoke());
                    mainPage.IsPresented = false;
                }),
            };
        }
    }
}
