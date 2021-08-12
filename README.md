# Introduction 
This project was created to explore the OpenTelemetry library, demonstrating its various distributed tracing capabilities through pocs along with different exporters such as Application Insights, Geneva, and Jaeger. 

# Getting Started
1. Geneva POC:
    - Visual Studio Dependency
    - .Net Core Framework dependency
    - refer to geneva documentation: https://eng.ms/docs/products/geneva/collect/instrument/opentelemetrydotnet/installation
2. Application Insights POC:
    - Visual Studio Dependency
    - .Net Core Framework dependency
    - refer to this tutorial: https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/telemetry-application-insights?pivots=programming-language-csharp
3. Jaeger POC:
    - Visual Studio Dependency
    - .Net Core Framework dependency
    - Requires Docker to be installed
4. For the Flask POC:
    - Requires Docker to be installed
    - Run the following commands:
        - pip install opentelemetry-api
        - pip install opentelemetry-sdk
        - pip install opentelemetry-exporter-jaeger
        - pip install opentelemetry-instrumentation-flask
        - pip install opentelemetry-instrumentation-requests

# Build and Test
- For the .Net POCs (App Insights, Geneva, Jaeger): 
    1. For Jaeger only:
        - In a separate terminal, run `docker run -p 16686:16686 -p 6831:6831/udp jaegertracing/all-in-one`
    2. Open the solution file in the WeatherForecast Folder
    3. Click Run
- For the Flask application:
    1. In a separate terminal, run `docker run -p 16686:16686 -p 6831:6831/udp jaegertracing/all-in-one`
    2. In another terminal, run `python main.py`
    3. Access https://localhost:16686.com to view traces in Jaeger
