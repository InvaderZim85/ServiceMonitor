# ServiceMonitor

The *ServiceMonitor* is a tiny REST service which checks a list of services and returns the result as a HTML page.

The REST service was created with:
- Nancy
- Topshelf

## Settings files
The service contains two settings files:

1. `Settings.json`
   
   Contains the settings for the service (Service name, Port, etc.)

2. `Services.json`

    Contains the list with the services which should be observed.