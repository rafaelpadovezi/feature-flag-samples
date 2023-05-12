import * as LDClient from 'launchdarkly-js-client-sdk';

const $ = document.querySelector.bind(document)

const context = {
  kind: 'user',
  key: 'user-"d821cbc0-2e4d-49fc-a5b4-990eb991beec"'
};
const client = LDClient.initialize('CLIENT_SIDE_ID', context);

const featureToggleHandler = () => {
  console.log(`Feature toggle handler has been called!`)
  const showEasterEgg = client.variation('SHOW_EASTER_EGG', false);
  if (showEasterEgg) {
    $(".feature-toggle-placeholder").style.display = "inline"
  } else {
    $(".feature-toggle-placeholder").style.display = "none"
  }
}

client.on("ready", featureToggleHandler)
client.on("change", featureToggleHandler)
