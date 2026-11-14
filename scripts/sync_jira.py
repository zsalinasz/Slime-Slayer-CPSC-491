import os
import requests
from requests.auth import HTTPBasicAuth
from collections import defaultdict

base_url = os.environ["JIRA_BASE_URL"].rstrip("/")
email = os.environ["JIRA_EMAIL"]
token = os.environ["JIRA_API_TOKEN"]
project_key = os.environ["JIRA_PROJECT_KEY"]

url = f"{base_url}/rest/api/3/search/jql"

params = {
    "jql": f'project = "{project_key}" ORDER BY status ASC, key ASC',
    "fields": "summary,status,assignee,priority",
    "maxResults": 100
}

response = requests.get(
    url,
    params=params,
    auth=HTTPBasicAuth(email, token),
    headers={"Accept": "application/json"},
    timeout=30
)

response.raise_for_status()

data = response.json()

statuses = defaultdict(list)

for issue in data.get("issues", []):
    key = issue["key"]
    fields = issue["fields"]

    summary = fields.get("summary", "")
    status = fields["status"]["name"]

    assignee = fields.get("assignee")

    if assignee:
        assignee_name = assignee["displayName"]
    else:
        assignee_name = "Unassigned"

    statuses[status].append({
        "key": key,
        "summary": summary,
        "assignee": assignee_name
    })

lines = [
    "# Jira Task Status",
    "",
    "> Automatically generated from Jira once per day.",
    ""
]

for status, issues in statuses.items():

    lines.append(f"## {status}")
    lines.append("")

    done_status = status.lower() in {
        "done",
        "closed",
        "completed"
    }

    for issue in issues:

        check = "x" if done_status else " "

        issue_url = (
            f"{base_url}/browse/{issue['key']}"
        )

        lines.append(
            f"- [{check}] "
            f"[{issue['key']}]({issue_url}) "
            f"- {issue['summary']} "
            f"— **Assignee:** {issue['assignee']}"
        )

    lines.append("")

with open(
    "JIRA_TASKS.md",
    "w",
    encoding="utf-8"
) as file:
    file.write("\n".join(lines))

print(
    f"Exported "
    f"{sum(len(v) for v in statuses.values())} "
    f"Jira issues."
)