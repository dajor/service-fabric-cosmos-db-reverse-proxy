---
services: service-fabric
platforms: dotnet
author: stvermas
---

# Service Fabric IoT Sample #

This sample project demonstrates how Service Fabric services that are exposed via a reverse proxy can be secured.

## Setup
  1. Set ApplicationGateway/Http/ForwardClientCertificate cluster setting to true. 
    https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cluster-fabric-settings

    SSL termination happens at the reverse proxy and all the client certificate data is lost. For the services to perform client certificate authentication, set the ForwardClientCertificate setting in the parameters section of ApplicationGateway/Http element.

    When ForwardClientCertificate is set to false, reverse proxy will not request for the client certificate during its SSL handshake with the client. This is the default behavior.
    When ForwardClientCertificate is set to true, reverse proxy requests for the client's certificate during its SSL handshake with the client. It will then forward the client certificate data in a custom HTTP header named X-Client-Certificate. The header value is the base64 encoded PEM format string of the client's certificate. The service can succeed/fail the request with appropriate status code after inspecting the certificate data. If the client does not present a certificate, reverse proxy forwards an empty header and let the service handle the case.

... TO BE CONTINUED


