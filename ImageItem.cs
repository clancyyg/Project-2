using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUICardsGUI
{
    public class ImageItem
    {
        public ImageSource ImageUrl { get; set; }
        public string Description { get; set; }

        public ImageItem(string imageName, string description)
        {
            ImageUrl = ImageSource.FromResource($"MAUICardsGUI.images.{imageName}", typeof(ImageItem).Assembly);

            Description = description;
        }
    }
}
