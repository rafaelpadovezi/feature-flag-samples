import requests
import json
import time
import argparse

def handle(api_key, project_key):
    with open('ffs.json') as ff_file:
        feature_flags = json.load(ff_file)

    headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': api_key
    }
    url = f"https://app.launchdarkly.com/api/v2/flags/{project_key}"

    for feature_flag in feature_flags:
        response = requests.request("POST", url, headers=headers, json=feature_flag)
        
        if 'X-Ratelimit-Route-Remaining' in response.headers:
            remaining = response.headers['X-Ratelimit-Route-Remaining']
            if remaining == "0":
                target_timestamp = response.headers['X-Ratelimit-Reset']

                current_timestamp = time.time()
                time_difference = int(target_timestamp)/1000 - current_timestamp

                if time_difference > 0:
                    print(f"Waiting {int(time_difference)} seconds until is safe to request again...")
                    time.sleep(time_difference)
            
        response.raise_for_status()
        print(f"{feature_flag['key']} added.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--api-key", type=str, required=True, help="LaunchDarkly API key"
    )
    parser.add_argument(
        "--project", type=str, default="default", help="Project key"
    )
    args = parser.parse_args()

    handle(args.api_key, args.project)