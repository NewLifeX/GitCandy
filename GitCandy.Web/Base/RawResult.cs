using System.Buffers;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace GitCandy.Web.Base;

public class RawResult : ActionResult
{
    public RawResult(Byte[] contents, String contentType = "text/plain", String fileDownloadName = null)
    {
        Contents = contents ?? throw new ArgumentNullException("contents");
        ContentType = contentType;
        FileDownloadName = fileDownloadName;
    }

    public Byte[] Contents { get; private set; }
    public String ContentType { get; private set; }
    public String FileDownloadName { get; private set; }

    public override void ExecuteResult(ActionContext context)
    {
        if (context == null)
            throw new ArgumentNullException("context");

        var response = context.HttpContext.Response;
        if (!String.IsNullOrEmpty(FileDownloadName))
            response.Headers.ContentDisposition = new ContentDisposition { FileName = FileDownloadName }.ToString();

        response.ContentType = ContentType;
        response.BodyWriter.Write(Contents);
    }
}