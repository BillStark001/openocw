

using System;
using System.Net;

namespace Oocw.Backend.Api;

public class ApiException(
    int statusCode = (int)HttpStatusCode.OK,
    int code = ApiResult.CODE_SUCCESS,
    string? message = null
) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
    public int Code { get; set; } = code;

    public ApiException((int, string?) codeDefinition) : this(
        (int)HttpStatusCode.OK,
        codeDefinition.Item1,
        codeDefinition.Item2
    ) {
    }
}