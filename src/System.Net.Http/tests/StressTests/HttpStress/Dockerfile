ARG SDK_BASE_IMAGE=mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster
FROM $SDK_BASE_IMAGE

WORKDIR /app
COPY . .

ENV STRESS_ARGS='-maxExecutionTime 30 -displayInterval 60'
CMD dotnet run -c Release -- $STRESS_ARGS