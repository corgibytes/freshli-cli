// This file was initially copied from https://github.com/grpc/grpc-dotnet/blob/948be08b088fbf449982b731c8e1a7cf8abb1965/examples/Tester/Tests/Client/Helpers/CallHelpers.cs
// It is covered by the license below.

// Some modifications have been added.

#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Threading.Tasks;
using Grpc.Core;

namespace Corgibytes.Freshli.Cli.Test.Common;

internal static class GrpcCalls
{
    // ReSharper disable once UnusedMember.Global
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(TResponse response)
    {
        return new AsyncUnaryCall<TResponse>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }

    // ReSharper disable once UnusedMember.Global
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(StatusCode statusCode)
    {
        var status = new Status(statusCode, string.Empty);
        return new AsyncUnaryCall<TResponse>(
            Task.FromException<TResponse>(new RpcException(status)),
            Task.FromResult(new Metadata()),
            () => status,
            () => new Metadata(),
            () => { });
    }

    public static AsyncServerStreamingCall<TResponse> CreateAsyncServerStreamingCall<TResponse>(
        IAsyncStreamReader<TResponse> responses)
    {
        return new AsyncServerStreamingCall<TResponse>(
            responses,
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { }
        );
    }
}
