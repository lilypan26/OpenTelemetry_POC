# Introduction 
This project was created to explore the OpenTelemetry library, specifically with a flask application. 
Based on following tutorial: https://opentelemetry-python.readthedocs.io/en/latest/getting-started.html

# Getting Started
1.	Run the following commands:
    - pip install opentelemetry-api
    - pip install opentelemetry-sdk
    - pip install opentelemetry-exporter-jaeger
    - pip install opentelemetry-instrumentation-flask
    - pip install opentelemetry-instrumentation-requests

# Build and Test
1. In a separate terminal, run `docker run -p 16686:16686 -p 6831:6831/udp jaegertracing/all-in-one`
2. In another terminal, run `python main.py`
3. Access https://localhost:16686.com to view traces in Jaeger
