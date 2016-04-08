using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.ViewModels;

namespace SocialNetwork.OAuth
{
    public class CustomViewService : DefaultViewService
    {
        public CustomViewService(DefaultViewServiceOptions config, 
            IViewLoader viewLoader) : base(config, viewLoader)
        { }

        public override Task<Stream> Error(ErrorViewModel model)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);

            writer.Write($"<h1>Oh no an error!</h1>{model.ErrorMessage}");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult<Stream>(stream);
        }
    }
}