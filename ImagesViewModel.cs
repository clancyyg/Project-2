using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MAUICardsGUI
{
    public class ImagesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ImageItem> Images { get; set; }

        public ImagesViewModel()
        {
            Images = new ObservableCollection<ImageItem> {
                new ImageItem ("card_clubs_02.png", "2C"),
                new ImageItem ("card_clubs_03.png", "3C"),
                // etc.
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
