using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading;
using Xamarin.Forms.Internals;
using System.Collections;

namespace QuickTest
{
    public static class ElementSearch
    {
        public static List<ElementInfo> Find(this Element element, Predicate<Element> predicate, Predicate<Element> containerPredicate = null)
        {
            var result = new List<ElementInfo>();

            if (containerPredicate != null && !containerPredicate.Invoke(element))
                return result;

            IEnumerable<ElementInfo> empty = new List<ElementInfo>();

            result.AddRange((element as Page)?.ToolbarItems.ToList().Where(predicate.Invoke).Select(ElementInfo.FromElement) ?? empty);
            result.AddRange((element as ContentPage)?.Content.Find(predicate, containerPredicate) ?? empty);
            result.AddRange((element as ContentView)?.Content.Find(predicate, containerPredicate) ?? empty);
            result.AddRange((element as ScrollView)?.Content.Find(predicate, containerPredicate) ?? empty);
            result.AddRange((element as Layout<View>)?.Children.ToList().SelectMany(child => child.Find(predicate, containerPredicate)) ?? empty);
            result.AddRange((element as ListView)?.Find(predicate, containerPredicate) ?? empty);

            if (predicate.Invoke(element))
                result.Add(ElementInfo.FromElement(element));

            AddTapGestureRecognizers(element, result);

            return result;
        }

        public static List<ElementInfo> Find(this Element element, string text)
        {
            if (text == null)
                throw new InvalidOperationException("Can't search for (null) text");

            return element.Find(e => e.HasText(text), c => (c as VisualElement)?.IsVisible ?? true);
        }

        public static bool HasText(this Element element, string text)
        {
            return
                (element as ToolbarItem)?.Text == text ||
                (element as ContentPage)?.Title == text ||
                (element as Button)?.Text == text ||
                (element as Label)?.Text == text ||
                (element as Label)?.FormattedText?.ToString() == text ||
                (element as Editor)?.Text == text ||
                (element as Entry)?.Text == text ||
                (element as SearchBar)?.Text == text ||
                ((element as Entry)?.Placeholder == text && string.IsNullOrEmpty((element as Entry)?.Text)) ||
                (element as TextCell)?.Text == text ||
                element?.AutomationId == text;
        }

        static void AddTapGestureRecognizers(Element sourceElement, IEnumerable<ElementInfo> result)
        {
            var tapGestureRecognizers = (sourceElement as View)?.GestureRecognizers.OfType<TapGestureRecognizer>().ToList();

            if (tapGestureRecognizers == null || !tapGestureRecognizers.Any())
                return;

            foreach (var info in result.Where(i => i.InvokeTap == null))
                info.InvokeTap = () => tapGestureRecognizers.ForEach(r => r.Invoke("SendTapped", sourceElement));
        }

        public static List<ElementInfo> Find(this ListView listView, Predicate<Element> predicate, Predicate<Element> containerPredicate)
        {
            var result = new List<ElementInfo>();

            result.AddRange(Find(listView.Header, predicate, containerPredicate));
            result.AddRange(Find(listView.Footer, predicate, containerPredicate));

            if (listView.ItemsSource == null)
                return result;

            if (listView.IsGroupingEnabled) {

                foreach (var currentGroup in listView.ItemsSource.Cast<IEnumerable<object>>().Select((v, i) => new { Value = v, Index = i })) {

                    var currentList = (TemplatedItemsList<ItemsView<Cell>, Cell>)((IList)listView.TemplatedItems)[currentGroup.Index];
                    var cell = currentList.HeaderContent;
                    result.AddRange(cell.Find(predicate, containerPredicate));

                    foreach (var item in currentGroup.Value.Select((v, i) => new { Value = v, Index = i })) {
                        var content = listView.ItemTemplate.CreateContent();
                        (content as Cell).BindingContext = item.Value;

                        var element = GetElement(predicate, containerPredicate, content);
                        if (element != null)
                            result.Add(new ElementInfo {
                                InvokeTap = () => listView.Invoke("NotifyRowTapped", currentGroup.Index, item.Index, null),
                                Element = element,
                            });
                    }
                }
            } else {
                foreach (var item in listView.ItemsSource.Cast<object>().Select((v, i) => new { Value = v, Index = i })) {
                    var content = listView.ItemTemplate.CreateContent();
                    (content as Cell).BindingContext = item.Value;

                    var element = GetElement(predicate, containerPredicate, content);
                    if (element != null)
                        result.Add(new ElementInfo {
                            InvokeTap = () => listView.Invoke("NotifyRowTapped", item.Index, null),
                            Element = element,
                        });
                }
            }

            return result;
        }

        static List<ElementInfo> Find(object stringBindingOrView, Predicate<Element> predicate, Predicate<Element> containerPredicate)
        {
            if (stringBindingOrView is string)
                return new Label { Text = stringBindingOrView.ToString() }.Find(predicate, containerPredicate);
            if (stringBindingOrView is View)
                return (stringBindingOrView as View).Find(predicate, containerPredicate);

            return new List<ElementInfo>();
        }

        static Element GetElement(Predicate<Element> predicate, Predicate<Element> containerPredicate, object cell)
        {
            if (predicate.Invoke(cell as Cell)) {
                return cell as Cell;
            }

            var viewCellResults = (cell as ViewCell)?.View.Find(predicate, containerPredicate);
            if (viewCellResults?.Any() ?? false) {
                return viewCellResults.First().Element;
            }

            return null;
        }
    }
}
