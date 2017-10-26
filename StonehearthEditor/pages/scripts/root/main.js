// Prevent MMB scroll to not interfere with curve scrolling.
$(window).mousedown(function (e) {
   if (e.button == 1) {
      e.preventDefault();
      e.stopPropagation();
   }
});

App = Ember.Application.create({
});

CsApi = {};

App.BlockPropertyComponent = Ember.Component.extend({
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.SubGroupPropertyComponent = Ember.Component.extend({
    classNames: ['sub-group-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level3ParameterPropertyComponent = Ember.Component.extend({
    classNames: ['level3-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level2ParameterPropertyComponent = Ember.Component.extend({
    classNames: ['level2-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level2ParameterPropertyEmissionComponent = Ember.Component.extend({
    classNames: ['level2-parameter-property-emission'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.OriginParameterPropertyComponent = Ember.Component.extend({
    classNames: ['origin-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.EqHelper = function (a) {
    return a[0] === a[1];
};

App.TostrHelper = function (a) {
    return a[0].toString();
};

App.WarningIconComponent = Ember.Component.extend({
    tagName: 'span',
});

App.CurveXComponent = Ember.Component.extend({
    width: 400,
    height: 150,
    topMargin: 5,
    bottomMargin: 20,
    leftMargin: 30,
    rightMargin: 30,
    curveSnapDistance: 10,
    colors: {
       primary: 'rgba(0, 127, 239, 0.75)',
       primaryHover: 'rgba(0, 127, 239, 1)',
       primaryLine: 'rgba(0, 50, 100, 1)',
       secondary: 'rgba(239, 127, 0, 0.75)',
       secondaryHover: 'rgba(239, 127, 0, 1)',
       secondaryLine: 'rgba(150, 70, 0, 1)',
       selected: 'rgba(255, 60, 60, 1)',
    },
    maxMsBetweenMergedChanges: 300,  // Changes to the same point this long apart are merged for undo/redo.
    didInsertElement: function () {
       var self = this;

       this._clampAndSortPoints();
       
       // Set up the canvas.
       var fullWidth = self.width + self.leftMargin + self.rightMargin;
       var fullHeight = self.height + self.topMargin + self.bottomMargin;
       self.svg = d3.select(self.$('svg').attr('class', 'curve-editor')[0])
            .attr('width', fullWidth)
            .attr('height', fullHeight)
            .on('dblclick', self._handleDoubleClick.bind(self))
            .on('mousedown', function () {
               if (d3.event.button == 0) {
                  self.set('selectedPoint', undefined);
                  self._update();
               }
            })
            .on('contextmenu', function () { d3.event.preventDefault(); })
            .call(d3.drag()
                     .filter(function () { return d3.event.button == 1; })
                     .on('start', self._handleDragStart.bind(self))
                     .on('drag', self._handleDrag.bind(self)));
       self.rect = self.svg.append('rect')
            .attr('width', fullWidth)
            .attr('height', fullHeight)
            .attr('fill', 'white');
       // Prevent browser scrolling with MMB - we're handling it in zoom.
       $(self.rect.node()).mousedown(function (e) {
          if (e.button == 1) {
             e.preventDefault();
          }
       });

       // Set up axes.
       self.xScale = d3.scaleLinear().domain([0, 1]).range([0, self.width]);
       self.baseYScale = d3.scaleLinear().domain(self._getRange()).range([self.height, 0]);
       self.yScale = self.baseYScale;
       self.xAxisTicks = d3.axisBottom(self.xScale).ticks(10);
       self.yAxisTicks1 = d3.axisLeft(self.yScale).ticks(5);
       self.yAxisTicks2 = d3.axisRight(self.yScale).ticks(5);
       self.xAxisView = self.svg.append('g')
            .attr('transform', 'translate(' + self.leftMargin + ',' + (self.height + self.topMargin) + ')')
            .call(self.xAxisTicks)
            .attr('style', 'pointer-events: none');
       self.yAxisView1 = self.svg.append('g')
            .attr('transform', 'translate(' + self.leftMargin + ',' + self.topMargin + ')')
            .call(self.yAxisTicks1)
            .attr('style', 'pointer-events: none');
       self.yAxisView2 = self.svg.append('g')
            .attr('transform', 'translate(' + (self.width + self.leftMargin) + ',' + self.topMargin + ')')
            .call(self.yAxisTicks2)
            .attr('style', 'pointer-events: none');

       // Set up Y axis pan/zoom on alt-MMB/wheel.
       self._resetZoom();
       self.$('.reframe').click(function () {
          // Undo current zoom.
          self.zoom.scaleTo(self.svg, 1);
          self.zoom.translateTo(self.svg, 0, self.height / 2);
          self.on('zoom', null);
          // Recalculate range.
          self.baseYScale = d3.scaleLinear().domain(self._getRange()).range([self.height, 0]);
          self.yScale = self.baseYScale;
          // Apply zoom.
          self._resetZoom();
          self._update();
       });

       // Set up containers for the points and lines. Separate to enforce z-index.
       self.linesView = self.svg.append('g')
            .attr('width', self.width)
            .attr('height', self.height)
            .attr('transform', 'translate(' + self.leftMargin + ',' + self.topMargin + ')')
            .attr('style', 'pointer-events: none');
       self.pointsView = self.svg.append('g')
            .attr('width', self.width)
            .attr('height', self.height)
            .attr('transform', 'translate(' + self.leftMargin + ',' + self.topMargin + ')');
       
       // Listen to point changes.
       self._getAllPoints().forEach(function (p) {
          p.addObserver('time', function () { self._handlePointChanged(this); });
          p.addObserver('value', function () { self._handlePointChanged(this); });
          // Cache old values for undo.
          p.set('_lastTime', p.time);
          p.set('_lastValue', p.value);
       });

       // Handle undo/redo.
       self._lastUpdateTime = 0;
       self._undoStack = [];  // Keys: curve, p, action ('add'/'remove'/'edit'), timeBefore, valueBefore, timeAfter, valueAfter
       self._undoIndex = -1;
       self._applyingUndoRedo = false;
       self.$('.undo').click(function () {
          if (self._undoIndex >= 0) {
             self._applyingUndoRedo = true;
             var operation = self._undoStack[self._undoIndex];
             if (operation.action == 'add') {
                operation.curve.points.removeObject(operation.p);
                self._update();
             } else if (operation.action == 'remove') {
                operation.curve.points.pushObject(operation.p);
                self.set('selectedPoint', operation.p);
                self._clampAndSortPoints();
                self._update();
             } else if (operation.action == 'edit') {
                operation.p.set('time', operation.timeBefore);
                operation.p.set('value', operation.valueBefore);
             }
             self._applyingUndoRedo = false;
             self._undoIndex--;
          }
       });
       self.$('.redo').click(function () {
          if (self._undoIndex < self._undoStack.length - 1) {
             self._undoIndex++;
             self._applyingUndoRedo = true;
             var operation = self._undoStack[self._undoIndex];
             if (operation.action == 'add') {
                operation.curve.points.pushObject(operation.p);
                self.set('selectedPoint', operation.p);
                self._clampAndSortPoints();
                self._update();
             } else if (operation.action == 'remove') {
                operation.curve.points.removeObject(operation.p);
                self._update();
             } else if (operation.action == 'edit') {
                operation.p.set('time', operation.timeAfter);
                operation.p.set('value', operation.valueAfter);
             }
             self._applyingUndoRedo = false;
          }
       });

       // Initial render.
       self._update();
    },
    _resetZoom: function () {
       var self = this;
       self.zoom = d3.zoom()
           .on('zoom', function () {
              var scaleY = d3.event.transform.k;
              self.yScale = d3.event.transform.rescaleY(self.baseYScale);
              self.yAxisView1.call(self.yAxisTicks1.scale(self.yScale));
              self.yAxisView2.call(self.yAxisTicks2.scale(self.yScale));
              self._update();
           })
           .filter(function () {
              return d3.event.altKey && (d3.event.type == 'wheel' || d3.event.button == 1);
           });
       self.svg.call(self.zoom);
    },
    _update: function () {
       var self = this;

       // Update the points.
       // See https://bost.ocks.org/mike/join/ for more details on this... interesting update approach.
       var points = this._getAllPoints();
       var pointsSelection = self.pointsView.selectAll('circle')
            .data(points);
       pointsSelection.exit()
            .remove();
       var newSelection = pointsSelection.enter()
            .append('circle')
            .attr('r', 4)
            .on("mouseover", function (p) {
               d3.select(this).attr('r', 8)
               if (p != self.get('selectedPoint')) {
                  d3.select(this).attr('fill', self.curve1.points.indexOf(p) >= 0 ? self.colors.primaryHover : self.colors.secondaryHover);
               }
            })
            .on("mouseout", function (p) {
               d3.select(this).attr('r', 4)
               if (p != self.get('selectedPoint')) {
                  d3.select(this).attr('fill', self.curve1.points.indexOf(p) >= 0 ? self.colors.primary : self.colors.secondary);
               }
            })
            .on('mousedown', self._handlePointMouseDown.bind(self))
            .on('contextmenu', self._handlePointRightClick.bind(self));
       pointsSelection.merge(newSelection)
            .attr('cx', function (p) {
               return self.xScale(p.time);
            })
            .attr('cy', function (p) {
               return self.yScale(p.value);
            })
            .attr('fill', function (p) {
               if (p == self.get('selectedPoint')) {
                  return self.colors.selected;
               } else {
                  return self.curve1.points.indexOf(p) >= 0 ? self.colors.primary : self.colors.secondary;
               }
            })
            .attr('stroke', function (p) {
               return p == self.get('selectedPoint') ? 'black' : 'none';
            })
            .attr('stroke-width', function (p) {
               return p == self.get('selectedPoint') ? 2 : 0;
            });

       // Replace the path.
       self.linesView.selectAll('path')
            .remove();
       var curves = [self.curve1];
       if (self.curve2) curves.push(self.curve2);
       curves.forEach(function (curve, i) {
          var points = curve.points.copy();
          if (points.length == 0) {
             points = [{ time: 0, value: 0 }];
          }
          if (points[0].time > 0) {
             points.unshift({ time: 0, value: points[0].value });
          }
          if (points[points.length - 1].time < 1) {
             points.push({ time: 1, value: points[points.length - 1].value });
          }
          self.linesView
               .append('path')
               .attr('d', d3.line()
                              .x(function (d) { return self.xScale(d.time); })
                              .y(function (d) { return self.yScale(d.value); })
                              .curve(d3.curveLinear)
                              (points))
               .attr('stroke', i ? self.colors.secondaryLine : self.colors.primaryLine)
               .attr('stroke-width', 1.5)
               .attr('fill', 'none');
       });

       // Draw the area between the curves of there are multiple.
       if (self.curve2) {
          var vertices = [];
          var ensureFillerVertex = function (curve, index, time) {
             if (!curve.points.length || curve.points[index].time > 0) {
                vertices.push({ time: time, value: curve.points[index] ? curve.points[index].value : 0 });
             }
          }

          // First curve.
          ensureFillerVertex(self.curve1, 0, 0);
          vertices = vertices.concat(self.curve1.points);
          ensureFillerVertex(self.curve1, self.curve1.points.length - 1, 1);

          // Second curve.
          ensureFillerVertex(self.curve2, self.curve2.points.length - 1, 1);
          vertices = vertices.concat(self.curve2.points.copy().reverse());
          ensureFillerVertex(self.curve2, 0, 0);

          // Replace the polygon.
          self.linesView.selectAll('polygon')
               .remove();
          self.linesView
               .append('polygon')
               .attr("points", function (d) {
                  return vertices.map(function (d) {
                     return [self.xScale(d.time), self.yScale(d.value)].join(",");
                  }).join(" ");
               })
               .attr('fill', 'rgba(0, 127, 239, 0.25)');
       }
    },
    _handleDoubleClick: function () {
       var self = this;
       var position = d3.mouse(self.pointsView.node());
       var curve = self.curve1;
       var curveNodes = self.linesView.selectAll('path').nodes();
       var projection = self._getClosestPointOnPath(curveNodes[0], position);
       if (self.curve2) {
          var distanceTo1 = projection.distance;
          var projection2 = self._getClosestPointOnPath(curveNodes[1], position);
          var distanceTo2 = projection2.distance;
          if (distanceTo2 < distanceTo1) {
             curve = self.curve2;
             projection = projection2;
          } else {
             // self is kinda flimsy, but I think it feels good.
             if (self.curve2.points.length < 2 && self.curve1.points.length >= 2 && distanceTo1 > self.curveSnapDistance) {
                curve = self.curve2;
                projection = projection2;
             }
          }
       }
       if (projection.distance <= self.curveSnapDistance) {
          position = projection;
       }
       var newPoint = Point.create({
          time: self.xScale.invert(position[0]),
          value: self.yScale.invert(position[1])
       });
       newPoint.addObserver('time', function () { self._handlePointChanged(this); });
       newPoint.addObserver('value', function () { self._handlePointChanged(this); });
       newPoint.set('_lastTime', newPoint.time);
       newPoint.set('_lastValue', newPoint.value);

       if (!self._applyingUndoRedo) {
          this._addUndoFrame({
             curve: curve,
             p: newPoint,
             action: 'add',
          });
       }

       curve.points.pushObject(newPoint);
       self.set('selectedPoint', newPoint);
       self._clampAndSortPoints();
       self._update();
    },
    _addUndoFrame: function (f) {
       while (this._undoIndex < this._undoStack.length - 1) {
          this._undoStack.pop();
       }
       this._undoStack.push(f);
       this._undoIndex = this._undoStack.length - 1;
    },
    _handlePointMouseDown: function (d) {
       this.set('selectedPoint', d);
       this._update();
       d3.event.stopPropagation();
       // Prevent browser scrolling with MMB - we're handling it in zoom.
       if (d3.event.button == 1) {
          d3.event.preventDefault();
       }
    },
    _handlePointChanged: function (p) {
       if (!this._applyingUndoRedo) {
          var isMergedUpdated = true;
          if (this._undoStack.length == 0) {
             isMergedUpdated = false;
          } else if (this._undoIndex != this._undoStack.length - 1) {
             isMergedUpdated = false;
          } else if (p != this._undoStack[this._undoStack.length - 1].p) {
             isMergedUpdated = false;
          } else if (this._undoStack[this._undoStack.length - 1].action != 'edit') {
             isMergedUpdated = false;
          } else if (Date.now() - this._lastUpdateTime > this.maxMsBetweenMergedChanges) {
             isMergedUpdated = false;
          }

          if (isMergedUpdated) {
             this._undoStack[this._undoStack.length - 1].timeAfter = p.time;
             this._undoStack[this._undoStack.length - 1].valueAfter = p.value;
          } else {
             this._addUndoFrame({
                curve: this.curve1.points.indexOf(p) >= 0 ? this.curve1 : this.curve2,
                p: p,
                action: 'edit',
                timeBefore: p._lastTime,
                valueBefore: p._lastValue,
                timeAfter: p.time,
                valueAfter: p.value,
             });
          }
       }
       p.set('_lastTime', p.time);
       p.set('_lastValue', p.value);

       this._lastUpdateTime = Date.now();

       this._clampAndSortPoints();
       this._update();
    },
    _handlePointRightClick: function (p) {
       var curve = this.curve1.points.indexOf(p) >= 0 ? this.curve1 : this.curve2;

       if (!self._applyingUndoRedo) {
          this._addUndoFrame({
             curve: this.curve1.points.indexOf(p) >= 0 ? this.curve1 : this.curve2,
             p: p,
             action: 'remove',
          });
       }

       curve.points.removeObject(p);
       this._update();
    },
    _handleDragStart: function () {
       if (!this.selectedPoint) return;
       this.dragOrigin = d3.mouse(this.pointsView.node());
       this.dragPointOrigin = [this.xScale(this.selectedPoint.time), this.yScale(this.selectedPoint.value)];
       this.dragKeepDimension = null;
    },
    _handleDrag: function () {
       if (!this.selectedPoint) return;
       var position = d3.mouse(this.pointsView.node());
       if (d3.event.sourceEvent.shiftKey) {
          if (!this.dragKeepDimension) {
             var dx = Math.abs(position[0] - this.dragOrigin[0]);
             var dy = Math.abs(position[1] - this.dragOrigin[1]);
             if (dx > dy) {
                this.dragKeepDimension = 'y';
             } else if (dx < dy) {
                this.dragKeepDimension = 'x';
             }
          }
       }
       // Update a subset of the dimensions.
       if (this.dragKeepDimension != 'x') {
          var newX = this.xScale.invert(this.dragPointOrigin[0] + position[0] - this.dragOrigin[0]);
          this.selectedPoint.set('time', Math.max(0, Math.min(newX, 1)));
       }
       if (this.dragKeepDimension != 'y') {
          var newY = this.yScale.invert(this.dragPointOrigin[1] + position[1] - this.dragOrigin[1]);
          this.selectedPoint.set('value', newY);
       }
       this._clampAndSortPoints();
       this._update();
    },
    _getAllPoints: function () {
       var points = [].concat(this.curve1.points);
       if (this.curve2) points = points.concat(this.curve2.points);
       return points;
    },
    _getRange: function () {
       var points = this._getAllPoints();
       if (points.length == 0) {
          return [0, 1];
       } else {
          var min = Infinity, max = -Infinity;
          points.forEach(function (p) {
             min = Math.min(min, p.value);
             max = Math.max(max, p.value);
          });
          if (min != max) {
             return [min, max];
          } else if (min >= 0 && min <= 1) {
            return [0, 1];
          } else {
             return [min / 2, min * 2];
          }
       }
    },
    _clampAndSortPoints: function () {
       this.curve1.points.forEach(function (p) {
          if (p.time < 0) p.set('time', 0);
          if (p.time > 1) p.set('time', 1);
       });
       this.curve1.points.sort(function (a, b) {
          return a.time - b.time;
       });
       if (this.curve2) {
          this.curve2.points.forEach(function (p) {
             if (p.time < 0) p.set('time', 0);
             if (p.time > 1) p.set('time', 1);
          });
          this.curve2.points.sort(function (a, b) {
             return a.time - b.time;
          });
       }
    },
    // From https://bl.ocks.org/mbostock/8027637, GPLv3
    _getClosestPointOnPath: function(pathNode, point) {
      var pathLength = pathNode.getTotalLength(),
          precision = 8,
          best,
          bestLength,
          bestDistance = Infinity;

      // linear scan for coarse approximation
      for (var scan, scanLength = 0, scanDistance; scanLength <= pathLength; scanLength += precision) {
         if ((scanDistance = distance2(scan = pathNode.getPointAtLength(scanLength))) < bestDistance) {
            best = scan, bestLength = scanLength, bestDistance = scanDistance;
         }
      }

      // binary search for precise estimate
      precision /= 2;
      while (precision > 0.5) {
         var before,
             after,
             beforeLength,
             afterLength,
             beforeDistance,
             afterDistance;
         if ((beforeLength = bestLength - precision) >= 0 && (beforeDistance = distance2(before = pathNode.getPointAtLength(beforeLength))) < bestDistance) {
            best = before, bestLength = beforeLength, bestDistance = beforeDistance;
         } else if ((afterLength = bestLength + precision) <= pathLength && (afterDistance = distance2(after = pathNode.getPointAtLength(afterLength))) < bestDistance) {
            best = after, bestLength = afterLength, bestDistance = afterDistance;
         } else {
            precision /= 2;
         }
      }

      best = [best.x, best.y];
      best.distance = Math.sqrt(bestDistance);
      return best;

      function distance2(p) {
         var dx = p.x - point[0],
             dy = p.y - point[1];
         return dx * dx + dy * dy;
      }
   }
});

App.CurveRgbComponent = Ember.Component.extend({
    actions: {
        add: function () {
            this.model.pointsRGB.pushObject(PointRgb.create({}));
        },
        addAbove: function (index) {
            this.model.pointsRGB.insertAt(index, PointRgb.create({}));
        },
        delete: function (index) {
            this.model.pointsRGB.removeAt(index);
        },
    },
});

App.BurstParameterPropertyComponent = Ember.Component.extend({
    actions: {
        add: function () {
            this.model.points.pushObject(PointBurst.create({}));
        },
        addAbove: function (index) {
            this.model.points.insertAt(index, PointBurst.create({}));
        },
        delete: function (index) {
            this.model.points.removeAt(index);
        },
    },
});

App.IndexController = Ember.Controller.extend({
    init: function () {
        this._super();
        var self = this;
        $(window).bind('keydown', function (event) {
            if (event.ctrlKey || event.metaKey) {
                var charCode = String.fromCharCode(event.which).toLowerCase();
                if (charCode === "s") {
                    self.save();
                }
            }
        });
    },
    save: function () {
        var jsonObj = this.get('model.effectModel').toJson();
        var json = Utils.formatEffectJson(jsonObj);
        EffectsJsObject.save(json);
    },
    actions: {
        preview: function () {
            EffectsJsObject.preview(this.get('model.effectModel').toJson());
        },
        save: function () {
            this.save();
        },
    },
});
