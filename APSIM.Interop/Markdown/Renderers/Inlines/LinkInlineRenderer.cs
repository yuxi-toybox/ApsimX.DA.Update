using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using APSIM.Shared.Utilities;
using Markdig.Syntax.Inlines;

namespace APSIM.Interop.Markdown.Renderers.Inlines
{
    /// <summary>
    /// This class renders a <see cref="HtmlInline" /> object to a PDF document.
    /// </summary>
    public class LinkInlineRenderer : PdfObjectRenderer<LinkInline>
    {
        /// <summary>
        /// Relative path to images.
        /// </summary>
        private string imageRelativePath;

        /// <summary>
        /// Construct a <see cref="LinkInlineRenderer"/> instance.
        /// </summary>
        /// <param name="imagePath">Relative path used to search for images. E.g. if an image URI is images/image.png, then the path will be assumed to be relative to this argument.</param>
        public LinkInlineRenderer(string imagePath)
        {
            imageRelativePath = imagePath;
        }

        /// <summary>
        /// Render the given LinkInline object to the PDF document.
        /// </summary>
        /// <param name="renderer">The PDF renderer.</param>
        /// <param name="link">The link object to be renderered.</param>
        protected override void Write(PdfBuilder renderer, LinkInline link)
        {
            string uri = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;
            if (link.IsImage)
            {
                Image image = GetImage(uri);
                renderer.AppendImage(image);
                // The assumption here is that any children of the image are the image's caption.
                renderer.StartNewParagraph();
                renderer.WriteChildren(link);
            }
            else
            {
                renderer.SetLinkState(uri);
                renderer.WriteChildren(link);
                renderer.ClearLinkState();
            }
        }

        /// <summary>
        /// Get the image specified by the given url.
        /// </summary>
        /// <param name="uri">Image URI.</param>
        private Image GetImage(string uri)
        {
            string path = PathUtilities.GetAbsolutePath(uri, imageRelativePath);
            if (File.Exists(path))
            {
                // Image.FromFile() will cause the file to be locked until the image is disposed of. 
                // This workaround allows us to immediately release the lock on the file.
                using (Bitmap bmp = new Bitmap(path))
                    return new Bitmap(bmp);
            }
            return APSIM.Services.Documentation.Image.LoadFromResource(uri);
        }
    }
}
