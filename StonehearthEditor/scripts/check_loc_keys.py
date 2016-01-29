#!/bin/env python
import os
import sys
import json
import re
from collections import OrderedDict
from pprint import pprint
from json_selector import JsonSelector

REGEX_PATTERN = re.compile('i18n\(stonehearth\:([^)]+)\)')
ROOT_PATH = "source/stonehearth_data/mods/stonehearth"
EN_JSON_PATH = "/locales/en.json"

ONLY_ALLOWED_FILES = [
]

SKIPPED_FILES = [
   'manifest.json',
   'en.json'
]

SUCCESS = False

class LocChecker:
   def __init__(self, stonehearth_root, json_file_root=None):
      global EN_JSON_PATH
      global ROOT_PATH
      if not json_file_root:
         json_file_root = ROOT_PATH
      self.json_file_root = json_file_root
      self.stonehearth_root = stonehearth_root
      self.root = stonehearth_root + json_file_root
      loc_file = open(self.root+EN_JSON_PATH, 'r')
      self.en_json = json.load(loc_file)

   def _check_string_field(self, field_value):
      if not isinstance(field_value, basestring):
         return False, "value " + str(field_value) + " is not a string!"

      if len(field_value) <= 0:
         # For now, ignore empty strings. BUT they might be a problem in the future.
         # TODO: Add support for which empty fields can't be ignored.
         return True, None

      match = REGEX_PATTERN.match(field_value)
      if match:
         key = match.group(1)
         key_path = key.split('.')
         parent = self.en_json
         for sub_path in key_path:
            if parent.get(sub_path):
               parent = parent.get(sub_path)
            else:
               return False, "String key " + field_value +" does not exist in en.json!" + str(parent)
         return True, None
      return False, "String " + field_value + " is not localized!"

   def check_json(self, file_list):
      passing = True
      required_loc_fields = open(self.stonehearth_root + 'scripts/i18n/loc_fields.json')
      LOCALIZABLE_FIELDS = json.load(required_loc_fields)

      for path in file_list:
         if not os.path.isfile(path):
            continue
         if not ROOT_PATH in path:
            continue
            
         if not self.json_file_root in path:
            continue

         root, f = os.path.split(path)
         if os.path.splitext(f)[1] != '.json':
            continue

         with open(path) as data_file:
            try:
               data = json.load(data_file)
            except Exception, e:
               print 'Improper Format: ' + path + str(e)
               passing = False

            print "checking file " + path
            file_errors = []
            json_selector = JsonSelector(data)
            for select_path in LOCALIZABLE_FIELDS['global'].keys():
               field_values = json_selector.select_path_values(select_path)
               for value in field_values:
                  result, message = self._check_string_field(value)
                  if not result:
                     file_errors.append(select_path + " Failed: " + message)
            if len(file_errors) > 0:
               print "file " + path + " has localization errors!  Run 'make loc' to automatically generate keys."
               for error in file_errors:
                  print "\t " + error
               passing = False

      return passing

   def get_all_files(self):
      global ROOT_PATH
      global ONLY_ALLOWED_FILES
      global SKIPPED_FILES
      files_to_be_parsed = []
      for root, dirs, files in os.walk(ROOT_PATH):
         root = root.replace('\\', "/")
         for f in files:
            if ONLY_ALLOWED_FILES and len(ONLY_ALLOWED_FILES) > 0 and not f in ONLY_ALLOWED_FILES:
               continue
            if f in SKIPPED_FILES:
               continue
            file_name_info = os.path.splitext(f)
            if file_name_info[1] == '.json':
               path = os.path.join(root, f)
               files_to_be_parsed.append(path)
      return files_to_be_parsed

if __name__ == "__main__":
   print "checking loc keys"
   loc_checker = LocChecker('C:/Radiant/stonehearth/', ROOT_PATH)
   loc_checker.check_json(loc_checker.get_all_files())
   print "check loc keys complete"