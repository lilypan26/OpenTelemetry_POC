# flask_example.py
import flask
import requests
import time

from opentelemetry import trace
from opentelemetry.instrumentation.flask import FlaskInstrumentor
from opentelemetry.instrumentation.requests import RequestsInstrumentor
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.exporter.jaeger.thrift import JaegerExporter
from opentelemetry.sdk.resources import SERVICE_NAME, Resource
from opentelemetry.sdk.trace.export import (
    BatchSpanProcessor,
    ConsoleSpanExporter,
    SimpleSpanProcessor,
)
#context = ('local.crt', 'local.key')

trace.set_tracer_provider(TracerProvider(
    resource=Resource.create({SERVICE_NAME: "flask-sample"})
))

jaeger_exporter = JaegerExporter(
    agent_host_name="localhost",
    agent_port=6831,
)

trace.get_tracer_provider().add_span_processor(
   BatchSpanProcessor(jaeger_exporter)
   #SimpleSpanProcessor(ConsoleSpanExporter())
)

# set_global_textmap(B3Format())

app = flask.Flask(__name__)
FlaskInstrumentor().instrument_app(app)
RequestsInstrumentor().instrument()
tracer = trace.get_tracer(__name__)


@app.route("/")
def hello():
    
    #base_span = tracer.start_span("example-request")
        #time.sleep(2)
        #requests.get("http://localhost:57033/api/values")
    requests.get("http://localhost:5006/newendpoint")
    #requests.get("http://localhost:57033/api/values")
    #requests.get("https://localhost:5001/")
    #base_span.end()
    return "hello world!"


@app.route("/newendpoint")
def second_endpoint():
    # tracer = trace.get_tracer(__name__)
    #with tracer.start_as_current_span("second-request"):
    #second_span = tracer.start_span("second-request")
        #time.sleep(2)
    #requests.get("http://localhost:57033/api/values")
    requests.get("https://google.com")
        #raise Exception('I know Python!')
    #second_span.end()
    return "hello world 2!"


app.run(debug=True, port=5006)