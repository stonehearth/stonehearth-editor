#!/bin/env python
import os
import sys
import json
import re
from collections import OrderedDict
from pprint import pprint
import argparse
from json_selector import JsonSelector

REGEX_PATTERN = re.compile('i18n\([^)]+\)')

ONLY_ALLOWED_FILES = [
]

SKIPPED_FILES = [
   'manifest.json',
   'en.json'
]

SKIPPED_FOLDERS = [
   'scripts/'
]

TRUNCATED_PATHS = {
   "data/gm/campaigns/goblin_war/arcs/trigger/raidcamp/encounters": "data/gm/campaigns/goblin_war",
   "data/gm/campaigns/goblin_war/arcs/climax/encounters": "data/gm/campaigns/goblin_war",
   "data/gm/campaigns/goblin_war/arcs/challenge/forward_scouts/encounters": "data/gm/campaigns/goblin_war",
   "data/gm/campaigns/ambient_threats/arcs/trigger/ambient_threats/encounters": "data/gm/campaigns/ambient_threats",
   "data/gm/campaigns/trader/arcs/encounters": "data/gm/campaigns/trader",
   "data/gm/campaigns/town_progression/arcs/trigger/town_progression/encounters":"data/gm/campaigns/town_progression",
   "data/gm/campaigns/titan_gary/arcs/trigger/titan_gary/encounters/":"data/gm/campaigns/titan",
   "data/gm/campaigns/titan/arcs/trigger/titan/encounters/":"data/gm/campaigns/titan"
}

INVALID_KEYWORDS = [
   "mixins",
   "mixintos"
]

TEMP_LOC_FILE = 'generated_loc.json'
EN_JSON_PATH = "/locales/"
EN_JSON_FILE = "en.json"

DEFAULT_ROOT_PATH = "source/stonehearth_data/mods"

# "/stonehearth/data/gm/campaigns/goblin_war/arcs"

def _print_info(string):
   if args.debug:
      print string

def get_files_to_be_parsed(mods_path):
   files_to_be_parsed = []
   if args.all:
      for root, dirs, files in os.walk(mods_path):
         root = root.replace('\\', "/")
         for f in files:
            files_to_be_parsed.append(os.path.join(root, f))         
   else:
      files_to_be_parsed = args.files
   return files_to_be_parsed

class LocGenerator:
   def __init__(self, root_path, mod, file_list):
      self.mod = mod
      self.file_list = file_list
      self.mod_path = root_path + "/" + self.mod
      self.python_location = os.path.realpath(os.path.join(os.getcwd(), os.path.dirname(__file__)))

   def generate_loc_keys(self):

      self.choice_regex_pattern = re.compile(self.mod + ':')

      if len(self.file_list) <= 0:
         return

      if args.dry_run:
         loc_file_path = 'generated_loc/'
      else:
         loc_file_path = self.mod_path + EN_JSON_PATH

      try:
         loc_file = open(loc_file_path + EN_JSON_FILE, 'r+')
         try:
            loc_dictionary = json.load(loc_file, object_pairs_hook=OrderedDict)
         except Exception as ex:
            print("\n==============")
            print("Invalid JSON in " + loc_file_path + EN_JSON_FILE)
            print("")
            template = "{0} exception. Arguments:\n{1!r}"
            message = template.format(type(ex).__name__, ex.args)
            print message
            print("==============\n")
           
            os._exit(777)
      except:
         if not os.path.exists(loc_file_path):
            os.makedirs(loc_file_path)
         loc_file = open(loc_file_path + EN_JSON_FILE, 'w')
         loc_dictionary = OrderedDict()

      loc_fields_data = open(os.path.join(self.python_location, 'loc_fields.json'))
      self.LOCALIZABLE_FIELDS = json.load(loc_fields_data, object_pairs_hook=OrderedDict)

      for path in self.file_list:
         if not self.mod_path in path:
            continue
         if not os.path.isfile(path):
            continue
         root, f = os.path.split(path)
         split_root = root.split(self.mod_path + "/")
         file_name_info = os.path.splitext(f)

         if len(split_root) > 1:
            split_root_modified = split_root[1]
            
            for original, replacement in TRUNCATED_PATHS.iteritems():
               if original == split_root_modified:
                  _print_info(original + ' is in ' + split_root_modified)
                  split_root_modified = replacement
                  break

            _print_info('split_root_modified ' + split_root_modified)
            localization_root = split_root_modified.split('/')
            
            _print_info('file name = ' + f + " last loc root = " + localization_root[-1])

            if file_name_info[0] != localization_root[-1]: 
               localization_root.append(file_name_info[0])
         else:
            localization_root = [file_name_info]

         if localization_root[0] in INVALID_KEYWORDS:
            localization_root[0] = self.mod + '_' + localization_root[0]

         _print_info("loc root " + str(localization_root))
         with open(path, 'r+') as data_file:
            print 'processing file: ' + path 
            modified = False
            data = json.load(data_file, object_pairs_hook=OrderedDict)

            json_selector = JsonSelector(data)
            for field, field_name_format in self.LOCALIZABLE_FIELDS['global'].iteritems():
               split_path = field.split('.')
               field_name = split_path.pop()

               found_parents = json_selector.select_path_parents(field)
               if len(found_parents) > 0:
                  _print_info("select path " + field + " returned " + str(found_parents))

               for unique_selector_path, found in found_parents.iteritems():
                  _print_info("unique_selector_path : " + unique_selector_path + " found: " + str(found) + " found type: " + str(type(found)))

                  if field_name == '*' and (not isinstance(found, basestring)):
                     if isinstance(found, dict):
                        for sub_name, item in found.iteritems():
                           has_mod = self._try_add_field(sub_name, localization_root, field_name_format, unique_selector_path + sub_name + '/', found, loc_dictionary)
                           if has_mod:
                              modified = True
                     else:
                        for i in range(0, len(found)):
                           sub_name = '{0:03d}'.format(i)
                           has_mod = self._try_add_field(sub_name, localization_root, field_name_format, unique_selector_path + sub_name + '/', found, loc_dictionary, found_parent_index=i)
                           if has_mod:
                              modified = True
                  elif isinstance(found, dict):
                     has_mod = self._try_add_field(field_name, localization_root, field_name_format, unique_selector_path, found, loc_dictionary)
                     if has_mod:
                        modified = True
                  else:
                     _print_info("Found a Non-dictionary parent for field " + unique_selector_path + ". Cannot convert into localized string.")

                  if modified and field == "components.unit_info.name":
                     found['display_name'] = found['name']
                     found.pop('name')

            # Do special formatting for choices
            for select_path, select_format in self.LOCALIZABLE_FIELDS['choices'].iteritems():
               parents = json_selector.select_path_parents(select_path)
               if len(parents) > 0:
                  for unique_selector_path, found in parents.iteritems():
                     count = 0
                     for key, value in found.iteritems():
                        sub_name = '{0:03d}'.format(count)
                        has_mod = self._try_add_choice(key, sub_name, localization_root, select_format, unique_selector_path, found, loc_dictionary)
                        if has_mod:
                           modified = True
                        count = count + 1

            if modified:
               _print_info("modifying file " + f)
               if not args.dry_run:
                  data_file_str = json.dumps(data, indent=3, separators=(',', ': '), ensure_ascii=False).encode('utf8')
                  data_file.seek(0)
                  data_file.truncate()
                  data_file.write(data_file_str)

      loc_file.seek(0)
      loc_file.truncate()
      json_string = json.dumps(loc_dictionary, indent=3, separators=(',', ': '), ensure_ascii=False).encode('utf8')
      loc_file.write(json_string)

      print 'localization key generation complete'

   # Helper function for adding a choice. Choices are different because the key is the stringkey of the loc string to use
   def _try_add_choice(self, found_field_name, desired_name, base_path, field_name_format, unique_selector_path, found_parent, loc_file_dictionary):
      if self.choice_regex_pattern.search(found_field_name):
         return False

      parent = loc_file_dictionary
      field_path_str = ""
      for loc_key in base_path:
         field_path_str = field_path_str + loc_key + "."
         if not parent.get(loc_key):
            parent[loc_key] = OrderedDict()
         parent = parent[loc_key]

      format_args = unique_selector_path.split('/')
      complete_field_name = field_name_format.format(*format_args) + desired_name
      additional_paths = complete_field_name.split('.')
      loc_file_field_name = additional_paths.pop()
      for path in additional_paths:
         field_path_str = field_path_str + path + "."
         if not parent.get(path):
            parent[path] = OrderedDict()
         parent = parent[path]

      if parent.get(loc_file_field_name):
         print "ERROR: field " + loc_file_field_name + " is already existent in parent: " + str(parent)
         return False

      parent[loc_file_field_name] = found_field_name
      localized_string_key = self.mod + ":" + field_path_str + loc_file_field_name
      found_parent[localized_string_key] = found_parent[found_field_name]
      found_parent.pop(found_field_name)
      return True

   def _try_add_field(self, field_name, base_path, field_name_format, unique_selector_path, found_parent, loc_file_dictionary, found_parent_index=-1):
      _print_info("Trying to add field " + field_name)

      if found_parent_index > -1:
         field_value = found_parent[found_parent_index]
      else:
         field_value = found_parent.get(field_name)

      if not isinstance(field_value, basestring):
         _print_info("Could not add " + field_name + " because its value was not a string.")
         return False

      if field_value and not REGEX_PATTERN.search(field_value):
         parent = loc_file_dictionary
         field_path_str = ""

         for loc_key in base_path:
            field_path_str = field_path_str + loc_key + "."
            if not parent.get(loc_key):
               parent[loc_key] = OrderedDict()
            parent = parent[loc_key]

         complete_field_name = field_name_format
         if unique_selector_path != '':
            
            fields = unique_selector_path.split('/')
            _print_info("field_name: " + complete_field_name + " unique_selector_path: " + str(fields))
            complete_field_name = complete_field_name.format(*fields)

         additional_paths = complete_field_name.split('.')
         loc_file_field_name = additional_paths.pop()
         for path in additional_paths:
            field_path_str = field_path_str + path + "."
            if not parent.get(path):
               parent[path] = OrderedDict()
            parent = parent[path]

         if parent.get(loc_file_field_name):
            print "ERROR: field " + loc_file_field_name + " is already existent in parent: " + str(parent)
            return False

         field_value.replace('\u2019', "'")

         parent[loc_file_field_name] = field_value
         localized_string_key = "i18n(" + self.mod + ":" + field_path_str + loc_file_field_name + ")"
         if found_parent_index > -1:
            found_parent[found_parent_index] = localized_string_key
         else:
            found_parent[field_name] = localized_string_key

         _print_info("Updating field value to: " + localized_string_key + " from: " + field_value)
         return True
      return False


   def _find_json_fields(self, json, field_path, unique_selector_path=''):
      #_print_info("trying to find subfield for field_path " + str(field_path) + " \nwithin json: " + str(json))
      found_parents = OrderedDict()
      parent = json
      is_valid = True

      for i in range(0, len(field_path)):
         key = field_path[i]
         if key == "*":
            #_print_info("got * for path " + str(field_path) + " at index " + str(i))
            subpath = field_path[i+1:]
            has_subpath = len(subpath) > 0
            if isinstance(parent, dict):
               for child_key, item in parent.iteritems():
                  updated_selector_path = unique_selector_path + child_key + '/';
                  if isinstance(item, dict):
                     if has_subpath:
                        sub_parents = _find_json_fields(item, subpath, unique_selector_path=updated_selector_path)
                        if sub_parents:
                           #_print_info("got subparents " + str(sub_parents))
                           found_parents.update(sub_parents)
                     else:
                        found_parents[updated_selector_path] = item
            else:
               for i in range(0, len(parent)):
                  child_key = '{0:03d}'.format(i)
                  updated_selector_path = unique_selector_path + child_key + '/';
                  if has_subpath:
                     sub_parents = _find_json_fields(parent[i], subpath, unique_selector_path=updated_selector_path)
                     if sub_parents:
                        #_print_info("got subparents " + str(sub_parents))
                        found_parents.update(sub_parents)
                  else:
                     found_parents[updated_selector_path] = parent[i]
         elif parent.get(key):
            parent = parent.get(key)
         else:
            is_valid = False
            break
      
      if is_valid:
         #_print_info("path is valid, adding itself: parent = " + str(parent) + " when name_prefix=" + name_prefix)
         found_parents[unique_selector_path] = parent

      #_print_info("found parents for path: " + str(field_path) + " \nare: " + str(found_parents))
      return found_parents

class GenerateLocKeys:
   def __init__(self, root_path, files):
      self.files_to_be_parsed = {}
      self.root_path = root_path
      for sample_file in files:
         root, f = os.path.split(sample_file)
         if ONLY_ALLOWED_FILES and not f in ONLY_ALLOWED_FILES:
            continue
         if f in SKIPPED_FILES:
            continue
         file_name_info = os.path.splitext(f)
         if file_name_info[1] != '.json':
            continue

         # Find the mod name
         file_directory = root.split(root_path)
         if len(file_directory) <= 1:
            continue
         mod = file_directory[1].split('/')[1]
            
         if not mod in self.files_to_be_parsed:
            self.files_to_be_parsed[mod] = []
         self.files_to_be_parsed[mod].append(sample_file)

   def process_files(self):
      for mod, file_list in self.files_to_be_parsed.iteritems():
         loc_generator = LocGenerator(self.root_path, mod, file_list)
         loc_generator.generate_loc_keys()

if __name__ == "__main__":
   parser = argparse.ArgumentParser(description='Stonehearth Auto Localization Key Generator.')
   parser.add_argument('-r', '--root', type=str, help='the root folder of all the mods for stonehearth')
   parser.add_argument('-d', '--debug', action='store_true', help='prints debug information about the key generation')
   parser.add_argument('-n', '--dry_run', action='store_true', help='do not actually write to the real files, so json files will not be modified and the keys will be written to ' + TEMP_LOC_FILE)
   parser.add_argument('-a', '--all', action='store_true', help='run the parser on everything in the mods folder. USE WITH CAUTION')
   parser.add_argument('files', metavar='F', type=str, nargs='*', help='a file for the generator to consider. Only files ending with .json will be parsed.')

   args = parser.parse_args(sys.argv[1:])
   loc_generator = GenerateLocKeys(args.root, get_files_to_be_parsed(args.root))
   loc_generator.process_files()
