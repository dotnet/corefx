FROM mcr.microsoft.com/dotnet-buildtools/prereqs:ubuntu-16.04-cross-10fcdcf-20190208200917 as corefx

WORKDIR /app
COPY . .

ARG ARCH=x64
ARG CONFIG=Release
RUN ./build.sh -c Release

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster

WORKDIR /app

ARG ARCH=x64
ARG CONFIG=Release
COPY --from=corefx /app/artifacts/bin/testhost/netcoreapp-Linux-${CONFIG}-${ARCH}/shared/Microsoft.NETCore.App/5.0.0/*.dll /usr/share/dotnet/shared/Microsoft.NETCore.App/3.0.0/
COPY --from=corefx /app/src/System.Net.Http/test/StressTests/HttpStress /repo

RUN dotnet run -c Release