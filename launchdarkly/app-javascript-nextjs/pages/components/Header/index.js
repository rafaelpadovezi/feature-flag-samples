import React, { useEffect, useState, useCallback } from "react"
import Link from "next/link"
import { useLaunchDarkly } from "../../contexts/feature-management"

const Header = () => {
  // States
  const [gameSharkMode, setGameSharkMode] = useState(false)
  const [easterEggDisplay, setEasterEggDisplay] = useState("none")
  // Feature toggle
  /**
   * @param client {LDClient.LDClient}
   */
  const configureBehaviorThroughFeatures = client => {
    setGameSharkMode(client.variation("GAME_SHARK_MODE"), false)
    setEasterEggDisplay(client.variation("SHOW_EASTER_EGG", false) ? "inline" : "none")
  }
  const whenNewConfigurationAvailableHandler = useCallback(client => {
    configureBehaviorThroughFeatures(client)
  })
  const client = useLaunchDarkly(whenNewConfigurationAvailableHandler)
  useEffect(() => {
    if (client) {
      configureBehaviorThroughFeatures(client)
    }
  }, [client])
  const projectTitle = gameSharkMode ? `Product QWERTY - GAME SHARK MODE` : `Product QWERTY`
  const easterEggTag = <span style={{ display: easterEggDisplay }}>👃</span>

  return (
    <header>
      <div className="d-flex flex-column flex-md-row align-items-center pb-3 mb-4 border-bottom">
        <Link href="/">
          <a href="#" className="d-flex align-items-center text-dark text-decoration-none">
            <span className="fs-4">
              {projectTitle} {easterEggTag}
            </span>
          </a>
        </Link>
        <nav className="d-inline-flex mt-2 mt-md-0 ms-md-auto">
          <Link href="/profile">
            <a className="me-3 py-2 text-dark text-decoration-none">Mocked profile</a>
          </Link>
          <Link href="/claims">
            <a className="me-3 py-2 text-dark text-decoration-none">Mocked claims</a>
          </Link>
          <a
            href="https://docs.getunleash.io/reference/sdks/javascript-browser"
            target="_blank"
            rel="noreferrer"
            className="me-3 py-2 text-dark text-decoration-none"
          >
            Know the truth
          </a>
        </nav>
      </div>
    </header>
  )
}

export default Header
