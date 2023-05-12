from random import choice

from django.contrib import admin
from ldclient import Context

from app_python_django.apps.core.models import Profile
from app_python_django.apps.core.providers.feature_management import client
from app_python_django.support.django_helpers import CustomModelAdminMixin


class CustomProfileAdminMixin:
    def get_queryset(self, request):
        user_id = "ffddfa83-0e0d-4eee-b091-0f3e1fe16773"
        user_context = {"key": f"user-{user_id}"}
        enable_profile_admin = client.variation("ENABLE_PROFILE_ADMIN", user_context, False)
        if not enable_profile_admin:
            return self.model.objects.none()
        return super().get_queryset(request)


@admin.register(Profile)
class ProfileAdmin(CustomModelAdminMixin, CustomProfileAdminMixin, admin.ModelAdmin):
    search_fields = ["mail"]
    list_filter = ["created_at", "updated_at", "sex"]
