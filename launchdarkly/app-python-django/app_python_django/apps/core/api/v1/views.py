from random import choice

from django.http import Http404
from rest_framework import filters
from rest_framework import viewsets

from app_python_django.apps.core.api.v1.serializers import CategorySerializer
from app_python_django.apps.core.api.v1.serializers import IngredientSerializer
from app_python_django.apps.core.api.v1.serializers import ProfileSerializer
from app_python_django.apps.core.models import Category
from app_python_django.apps.core.models import Ingredient
from app_python_django.apps.core.models import Profile
from app_python_django.apps.core.providers.feature_management import client


class ProfileViewSet(viewsets.ModelViewSet):
    queryset = Profile.objects.all()
    serializer_class = ProfileSerializer
    filter_backends = [filters.SearchFilter]
    search_fields = ["username", "sex", "mail"]

    def initial(self, request, *args, **kwargs):
        """
        https://www.django-rest-framework.org/api-guide/views/#initialself-request-args-kwargs
        """

        users = [
            "40956364-e486-4d8e-b35e-60660721f467",
            "882c4eb4-7c89-4998-b93d-9efce0e7270c",
            "64d8eb17-ae09-403e-be65-786dae090750",
            "ffddfa83-0e0d-4eee-b091-0f3e1fe16773",
        ]
        user_context = {"key": f"user-{choice(users)}"}
        enable_profile_api = client.variation("ENABLE_PROFILE_API", user_context, False)
        if not enable_profile_api:
            raise Http404("Not available")
        super().initial(request, *args, **kwargs)


class CategoryViewSet(viewsets.ModelViewSet):
    queryset = Category.objects.all()
    serializer_class = CategorySerializer


class IngredientViewSet(viewsets.ModelViewSet):
    queryset = Ingredient.objects.all()
    serializer_class = IngredientSerializer
