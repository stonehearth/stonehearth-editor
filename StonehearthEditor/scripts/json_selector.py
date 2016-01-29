#!/bin/env python
import os
import sys
import json
import re
from collections import OrderedDict
from pprint import pprint

class JsonSelector:
   def __init__(self, json):
      self.json = json
      self._selector_cache = {}

   def _fields_generator(self, field):
      if isinstance(field, dict):
         for child_key, item in field.iteritems():
            yield child_key, item
      else:
         for i in range(0, len(field)):
            child_key = '{0:03d}'.format(i)
            yield child_key, field[i]

   def _select_parent_path_internal(self, node, path, parent_path=''):
      self._debug_print("trying to find subfield for field_path " + str(path))

      found = OrderedDict()

      if len(path) <= 0:
         found[parent_path] = node
         return found

      if not isinstance(node, dict) and not isinstance(node, list):
         self._debug_print("dead end. Node = " + str(node))
         return found

      subpath = list(path)
      field_name = subpath.pop(0)
      has_subpath = len(path) > 0
      if field_name == "*":
         for child_key, child in self._fields_generator(node):
            self._debug_print("Child key = " + child_key)
            updated_selector_path = parent_path + child_key + '/';
            if has_subpath:
               child_found = self._select_parent_path_internal(child, subpath, parent_path=updated_selector_path)
               found.update(child_found)
            else:
               found[updated_selector_path] = child
      else:
         if not isinstance(node, dict):
            self._debug_print("dead end. Trying to get specific field name for " + field_name + " when node is not a dictionary")
            return found

         child = node.get(field_name)
         if child:
            child_found = self._select_parent_path_internal(child, subpath, parent_path=parent_path)
            found.update(child_found)

      return found
      
   def _get_field_name_and_path(self, path_str):
      path = path_str
      last_field = path.rfind('.')
      field_name = path
      if last_field >= 0:
         field_name = path[last_field + 1:]
         path = path[0: last_field]
      else:
         path = ""

      return field_name, path

   def select_path_parents(self, path_str):
      self._debug_print("Running select path on path:" + path_str)
      field_name, path = self._get_field_name_and_path(path_str)

      solution = self._selector_cache.get(path)
      if solution:
         self._debug_print("Hit the cache!")
      else:
         if len(path) > 0:
            split_path = path.split('.')
         else:
            split_path = []
         solution = self._select_parent_path_internal(self.json, split_path)
         self._selector_cache[path] = solution

      parsed_solution = dict(solution)
      isValidSolution = False
      for unique_selector_path, parent in solution.iteritems():
         if field_name == '*' and (not isinstance(parent, basestring)):
            isValidSolution = True
         elif isinstance(parent, dict) and isinstance(parent.get(field_name), basestring):
            isValidSolution = True
         else:
            isValidSolution = False
         if not isValidSolution:
            self._debug_print(field_name + " is not a valid solution of " + str(parent) + ". removing ")
            parsed_solution.pop(unique_selector_path)

      return parsed_solution

   def select_path_values(self, path_str):
      field_name, path = self._get_field_name_and_path(path_str)
      fields = []
      parents = self.select_path_parents(path_str)
      for _, parent in parents.iteritems():
         if field_name == '*':
            for _, item in self._fields_generator(parent):
               fields.append(item)
         else:
            item = parent.get(field_name)
            fields.append(parent.get(field_name))
      return fields

   def _debug_print(self, message):
      #print message
      return