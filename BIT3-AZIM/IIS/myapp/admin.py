from django.contrib import admin
from django.apps import apps


# Register your models here.


#admin.site.register(Question)
#admin.site.register(Choice)
app = apps.get_app_config('myapp')


for model_name, model in app.models.items():
    admin.site.register(model)

