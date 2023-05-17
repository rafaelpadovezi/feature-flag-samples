import React, { useEffect } from "react"
import "bootstrap/dist/css/bootstrap.css"
import { LaunchDarklyProvider } from "./contexts/feature-management"

function MyApp({ Component, pageProps }) {
  useEffect(() => {
    import("bootstrap/dist/js/bootstrap")
  }, [])

  useEffect(() => {
    typeof document !== undefined ? require("bootstrap/dist/js/bootstrap") : null
  }, [])

  return (
    <LaunchDarklyProvider>
      <Component {...pageProps} />
    </LaunchDarklyProvider>
  )
}

export default MyApp
