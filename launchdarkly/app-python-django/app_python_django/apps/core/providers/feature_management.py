import logging

import ldclient

from django.conf import settings
from ldclient.config import Config
from ldclient.integrations import Files

logger = logging.getLogger(__name__)


def setting(name, default=None):
    """
    Helper function to get a Django setting by name. If setting doesn't exist it will return a default.
    """
    return getattr(settings, name, default)


class Client:
    def __init__(self):
        self._launch_darkly_sdk_key = setting("LAUNCH_DARKLY_SDK_KEY")
        self._verbose_log_level = setting("LAUNCH_DARKLY_VERBOSE_LOG_LEVEL", logging.WARNING)
        self._is_online = setting("LAUNCH_DARKLY_ONLINE", False)
        self._flag_data_file_path = setting("LAUNCH_DARKLY_FLAG_DATA_FILE_PATH")
        self._flush_interval = setting("LAUNCH_DARKLY_FLUSH_INTERVAL", 5)
        # The number of seconds between polls for flag updates if streaming is off.
        self._poll_interval = setting("LAUNCH_DARKLY_POLL_INTERVAL", 30)
        # private attributes are not sent to launchdarkly
        self._private_attributes = ["cpf", "email"]

    def connect(self):
        if self._is_online:
            config = Config(
                self._launch_darkly_sdk_key,
                poll_interval=self._poll_interval,
                flush_interval=self._flush_interval,
                application={"id": "app-python-django"},
                private_attributes=self._private_attributes
            )
        else: # only for tests and local development
            data_source_callback = Files.new_data_source(paths=[self._flag_data_file_path], auto_update=True)
            config = Config(self._launch_darkly_sdk_key, update_processor_class=data_source_callback, send_events=False)
            logger.debug("Loading feature flags from a file")

        ldclient.set_config(config)

        ld_logger = logging.getLogger("ldclient")
        ld_logger.setLevel(self._verbose_log_level)

        client = ldclient.get()

        if not client.is_initialized():
            logger.error("LaunchDarkly SDK was not initialized and the app is running with default values")

        self._client = client
        return client


# https://docs.getunleash.io/unleash-client-python/usage.html
client = Client().connect()
