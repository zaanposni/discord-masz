"""
migrate_dyno_warnings.py
this scripts parses all dynobot warnings registered to your guild into useable SQL Code that can be injected into the database.
"""

import json
import os
import argparse
from datetime import datetime

import dateparser

def load_arguments():
    """
    Arguments to the program.
    :return: An objects with argument name properties
    """
    parser = argparse.ArgumentParser()
    parser.add_argument('--input-file', help='relative path to input file', default="./input.json")
    parser.add_argument('--output-file', help='relative path to output file', default="./output.sql")
    return parser.parse_args()


START_ARGS = load_arguments()
PATH_TO_FILE = os.path.join(".", START_ARGS.input_file)
OUTPUT_FILE = os.path.join(".", START_ARGS.output_file)

if not os.path.isfile(PATH_TO_FILE):
    print(f"{PATH_TO_FILE} not found.")
    exit(1)


with open(PATH_TO_FILE, "r", encoding="utf-8") as fh:
    dyno_data = json.load(fh)

try:
    dyno_logs = dyno_data["logs"]
except KeyError:
    print("Key 'logs' not found in json. Either your input is invalid or this script is out of date.")
    exit(1)

SQL_STRING = ""
NEW_ENTRY = 'INSERT INTO ModCases (GuildId, CaseId, CreatedAt, Description, Labels, LastEditedAt, LastEditedByModId, ModId, ' \
                                   'Nickname, OccuredAt, Punishment, Severity, Title, UserId, Username, Valid, PunishedUntil, PunishmentActive, PunishmentType)' \
                                   ' VALUES("{}", {}, "{}", "{}", "dyno-import", "{}", "{}", "{}", "{}", "{}", "None", 0, "Imported from Dyno", "{}", "{}", 1, null, false, 0);'
for e, case in enumerate(dyno_logs, 1):
    #date = datetime.strptime(case["createdAt"], "%a, %b %-d, %Y %-I:%M %p").isoformat()
    date = dateparser.parse(case["createdAt"]).isoformat()
    string = NEW_ENTRY.format(case["guild"], e, date, case["reason"].replace('"', "'") + f"\nDynobot case id: '{case['id']}'", date, case["mod"]["id"], case["mod"]["id"],
                              case["user"]["username"].replace('"', "'"), date, case["user"]["id"], case["user"]["username"].replace('"', "'"))
    SQL_STRING += f"{string}\n"

    print(string)

with open(os.path.join(".", OUTPUT_FILE), "w", encoding="utf-8") as fh:
    fh.write(SQL_STRING)

print("Done.")
