import React, { createContext, useContext, useEffect, useState } from "react"
import * as LDClient from 'launchdarkly-js-client-sdk';
import { useUserAgent } from "../hooks/user_agent"


const LaunchDarklyContext = createContext(null)
/**
 * @param onConfigurationUpdate {Function}
 * @returns {LDClient.LDClient || null}
 */
const useLaunchDarkly = (onConfigurationUpdate = null, flagName = '') => {
  const launchDarkly = useContext(LaunchDarklyContext)
  useEffect(() => {
    if (launchDarkly && onConfigurationUpdate) {
      const changeEvent = flagName ? `change:${flagName}` : 'change'
      launchDarkly.on(changeEvent, () => onConfigurationUpdate(launchDarkly))
    }
  }, [onConfigurationUpdate, flagName, launchDarkly])

  return launchDarkly
}

const LaunchDarklyProvider = ({ children }) => {
  const [launchDarkly, setLaunchDarkly] = useState(null)
  const userAgent = useUserAgent()

  useEffect(() => {
    // In launchdarkly the client is initialized with the context.
    // Is possible to change it using identify.
    // For now, let's mock the user ID!
    let userId = "40956364-e486-4d8e-b35e-60660721f467"
    if (userAgent) {  
      const userIdPersonalComputer = "40956364-e486-4d8e-b35e-60660721f467"
      const userIdMobile = "d821cbc0-2e4d-49fc-a5b4-990eb991beec"
      userId = userAgent.device.isMobile ? userIdMobile : userIdPersonalComputer
    }
    const context = {
      kind: 'user',
      key: `user-${userId}`,
      email: 'test@example.com'
    }
    const client = LDClient.initialize(
      process.env.NEXT_PUBLIC_LAUNCHDARKLY_CLIENT_ID,
      context,
      {
        // Store and load feature flags from localStorage
        // https://docs.launchdarkly.com/sdk/features/bootstrapping#javascript
        bootstrap: 'localStorage',
        privateAttributes: ['email']
      },
    )
    client.waitForInitialization().then(() => {
      setLaunchDarkly(client)
    }).catch(err => {
      console.error("LaunchDarkly didn't initialize correctly! Only default values will be considered: " + err)
    });

    return () => {
      return client.close();
    }
  }, [])

  return <LaunchDarklyContext.Provider value={launchDarkly}>{children}</LaunchDarklyContext.Provider>
}

export { LaunchDarklyProvider, useLaunchDarkly }
