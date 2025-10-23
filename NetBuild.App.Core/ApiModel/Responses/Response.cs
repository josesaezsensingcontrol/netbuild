namespace NetBuild.App.Core.ApiModel.Responses
{
    public class Response<T> : Response
    {
        public T Data { get; set; }
    }

    public class Response
    {
        public Response()
        {
            Errors = new List<string>();
        }

        public bool IsSuccess => !Errors.Any();
        public List<string> Errors { get; set; }
    }
}
