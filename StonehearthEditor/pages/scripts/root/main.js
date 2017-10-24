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
    colors: {
       primary: 'rgba(0, 127, 239, 0.75)',
       primaryHover: 'rgba(0, 127, 239, 1)',
       primaryLine: 'rgba(0, 50, 100, 1)',
       secondary: 'rgba(239, 127, 0, 0.75)',
       secondaryHover: 'rgba(239, 127, 0, 1)',
       secondaryLine: 'rgba(150, 70, 0, 1)'
    },
    didInsertElement: function () {
       var self = this;

       this._sortPoints();
       
       // Set up the canvas.
       var fullWidth = self.width + self.leftMargin + self.rightMargin;
       var fullHeight = self.height + self.topMargin + self.bottomMargin;
       self.svg = d3.select(self.$('svg').attr('class', 'curve-editor')[0])
            .attr('width', fullWidth)
            .attr('height', fullHeight)
            .on('dblclick', self._handleDoubleClick.bind(self))
            .on('contextmenu', function () { d3.event.preventDefault(); });
       self.rect = self.svg.append('rect')
            .attr('width', fullWidth)
            .attr('height', fullHeight)
            .attr('fill', 'white');

       // Set up axes.
       self.xScale = d3.scaleLinear().domain([0, 1]).range([0, self.width]);
       self.baseYScale = d3.scaleLinear().domain(self._getRange()).range([self.height, 0]);
       self.yScale = self.baseYScale;
       self.xAxisTicks = d3.axisBottom(self.xScale).ticks(10);
       self.yAxisTicks = d3.axisLeft(self.yScale).ticks(5);
       self.xAxisView = self.svg.append('g')
            .attr('transform', 'translate(' + self.leftMargin + ',' + (self.height + self.topMargin) + ')')
            .call(self.xAxisTicks)
            .attr('style', 'pointer-events: none');
       self.yAxisView1 = self.svg.append('g')
            .attr('transform', 'translate(' + self.leftMargin + ',' + self.topMargin + ')')
            .call(self.yAxisTicks)
            .attr('style', 'pointer-events: none');
       self.yAxisView2 = self.svg.append('g')
            .attr('transform', 'translate(' + (self.width + self.leftMargin) + ',' + self.topMargin + ')')
            .call(self.yAxisTicks)
            .attr('style', 'pointer-events: none');

       // Set up Y axis pan/zoom on Ctrl-wheel/Ctrl-drag.
       self._resetZoom();
       self.$('.zoom').click(function () {
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
       
       // Initial render.
       self._update();
    },
    _resetZoom: function () {
       var self = this;
       self.zoom = d3.zoom()
           .on('zoom', function () {
              var scaleY = d3.event.transform.k;
              self.yScale = d3.event.transform.rescaleY(self.baseYScale);
              self.yAxisView1.call(self.yAxisTicks.scale(self.yScale));
              self.yAxisView2.call(self.yAxisTicks.scale(self.yScale));
              self._update();
           })
           .filter(function () {
              return !d3.event.button && (d3.event.buttons == 1 || d3.event.ctrlKey);
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
               d3.select(this)
                    .attr('r', 8)
                    .attr('fill', self.curve1.points.indexOf(p) >= 0 ? self.colors.primaryHover : self.colors.secondaryHover);
            })
            .on("mouseout", function (p) {
               d3.select(this)
                    .attr('r', 4)
                    .attr('fill', self.curve1.points.indexOf(p) >= 0 ? self.colors.primary : self.colors.secondary);
            })
            .on('contextmenu', self._handlePointRightClick.bind(self))
            .call(d3.drag()
                     .on('drag', self._handleDrag.bind(self)));
       pointsSelection.merge(newSelection)
            .attr('cx', function (p) {
               return self.xScale(p.time);
            })
            .attr('cy', function (p) {
               return self.yScale(p.value);
            })
            .attr('fill', function (p) {
               return self.curve1.points.indexOf(p) >= 0 ? self.colors.primary : self.colors.secondary;
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

          // Repalce the polygon.
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
       var position = d3.mouse(this.pointsView.node());
       var curve = this.curve1;
       if (this.curve2) {
          var curves = this.linesView.selectAll('path').nodes();
          var distanceTo1 = this._getClosestPointOnPath(curves[0], position).distance;
          var distanceTo2 = this._getClosestPointOnPath(curves[1], position).distance;
          if (distanceTo2 < distanceTo1) {
             curve = this.curve2;
          } else {
             // This is kinda flimsy, but I think it feels good.
             if (this.curve2.points.length < 2 && this.curve1.points.length >= 2 && distanceTo1 > 5) {
                curve = this.curve2;
             }
          }
       }
       curve.points.pushObject(Point.create({
          time: this.xScale.invert(position[0]),
          value: this.yScale.invert(position[1])
       }));
       this._sortPoints();
       this._update();
    },
    _handlePointRightClick: function (d) {
       // One of these will succeed.
       this.curve1.points.removeObject(d);
       if (this.curve2) this.curve2.points.removeObject(d);

       this._update();
    },
    _handleDrag: function (d) {
       var position = d3.mouse(this.pointsView.node());
       d.time = Math.max(0, Math.min(this.xScale.invert(position[0]), 1));
       d.value = this.yScale.invert(position[1]);
       this._sortPoints();
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
    _sortPoints: function () {
       this.curve1.points.sort(function (a, b) {
          return a.time - b.time;
       });
       if (this.curve2) {
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
