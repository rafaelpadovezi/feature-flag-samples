# App Python Django with LaunchDarkly SDK

> The original project made with unleash can be found at https://github.com/willianantunes/tutorials/tree/master/2023/05/feature_toggle_unleash/app-python-django

This project demonstrates how to use feature toggles with LaunchDarkly, either in the backend or frontend side.

## Configuration

- [Backend](./.env.development)
- [Frontend](./app_python_django/apps/core/static/core/js/app.js)

## Feature toggles

Where you can see feature toggles:

- [app_python_django/apps/core/views.py](./app_python_django/apps/core/views.py)
- [app_python_django/apps/core/templates/core/templates/header.html](./app_python_django/apps/core/templates/core/templates/header.html)
- [app_python_django/apps/core/templates/core/pages/home.html](./app_python_django/apps/core/templates/core/pages/home.html)
- [app_python_django/apps/core/admin.py](./app_python_django/apps/core/admin.py)
- [app_python_django/apps/core/static/core/js/app.js](./app_python_django/apps/core/static/core/js/app.js)

### Local development

The project was created and tested during the trial but it's possible to run the project and see the feature flags working [loading the flags from a file](https://docs.launchdarkly.com/sdk/features/flags-from-files).

- [launchdarkly-flagdata.json](./launchdarkly-flagdata.json)

### Explore de API

How to list all profiles:

    http GET :8000/api/v1/profile/

How to create a profile:

    http POST :8000/api/v1/profile/ name=Jafar address=Agrabah mail=jafar@agrabah.com sex=M username=jafar

How to find records:

    http GET :8000/api/v1/profile/ search==Jafar
