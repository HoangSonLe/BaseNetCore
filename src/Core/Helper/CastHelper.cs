using Core.CommonModels.BaseModels;

namespace Core.Helper
{
    public static class CastHelper
    {
        public static Acknowledgement AsAcknowledgement(this Acknowledgement ackT)
        {
            return new Acknowledgement()
            {
                StatusCode = ackT.StatusCode,
                ErrorMessageList = ackT.ErrorMessageList,
                SuccessMessageList = ackT.SuccessMessageList
            };
        }

        public static Acknowledgement<TResult> AsAcknowledgement<TResult>(this Acknowledgement ackTSource)
        {
            return new Acknowledgement<TResult>()
            {
                StatusCode = ackTSource.StatusCode,
                ErrorMessageList = ackTSource.ErrorMessageList,
                SuccessMessageList = ackTSource.SuccessMessageList
            };
        }
    }
}
