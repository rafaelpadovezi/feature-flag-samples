import logging

from django.shortcuts import redirect
from django.shortcuts import render
from django.urls import reverse
from ldclient import Context

from app_python_django.apps.core.models import Profile
from app_python_django.apps.core.providers.feature_management import client

logger = logging.getLogger(__name__)


def index(request):
    user_id_pc = "40956364-e486-4d8e-b35e-60660721f467"
    user_id_mobile = "d821cbc0-2e4d-49fc-a5b4-990eb991beec"
    user_id = user_id_pc if request.user_agent.is_pc else user_id_mobile
    user_context = (
        Context.builder(f"user-{user_id}")
        .set("userId", user_id)
        .set("browser", request.user_agent.browser.family.lower())
        .build()
    )

    # `PERMISSION` TOGGLES
    should_show_profiles = client.variation("SHOW_PROFILES", user_context, False)
    should_allow_profile_management = client.variation("ALLOW_PROFILE_MANAGEMENT", user_context, False)
    # `EXPERIMENT` TOGGLES
    profile_management_button_scheme = client.variation_detail(
        "PROFILE_MANAGEMENT_BUTTON_SCHEME", user_context, "btn-danger"
    )
    button_scheme_reason = profile_management_button_scheme.reason["kind"]
    button_scheme_value = profile_management_button_scheme.value

    logger.info("Using the button scheme with value %s due to %s", button_scheme_value, button_scheme_reason)

    text_presentation_fallback = {
        "profileTitle": "Registered profiles",
        "subTitle": "Change how this app behave by changing the feature toggle tool ⚒",
        "title": "Hello there 😄!",
    }
    text_presentation_value = client.variation("TEXT_PRESENTATION", user_context, text_presentation_fallback)

    # `KILL-SWITCH TOGGLES
    game_shark_mode = client.variation("GAME_SHARK_MODE", user_context, False)

    context = {
        "show_profiles": should_show_profiles,
        "allow_profile_management": should_allow_profile_management,
        "button_scheme_value": button_scheme_value,
        "text_presentation": text_presentation_value,
        "game_shark_mode": game_shark_mode,
        "profiles": Profile.objects.all(),
    }

    return render(request, "core/pages/home.html", context)


def manage_profiles(request):
    redirect_uri = _build_uri(request, "index")

    if request.method == "POST" and request.POST.get("method") == "DELETE":
        profile_id = request.POST["profileId"]
        Profile.objects.get(id=profile_id).delete()

    return redirect(redirect_uri)


def _build_uri(request, view_name):
    location_redirect = reverse(view_name)
    return request.build_absolute_uri(location_redirect)
