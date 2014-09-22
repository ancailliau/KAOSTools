var RenderGoalDiagram = function (data, selector) {

  var nodes = [];
  var edges = [];
  
  var containsNode = function (identifier) {
    for(var i = 0; i < nodes.length; i++) {
      if (nodes[i].value.Identifier == identifier) {
        return true;
      }
    }
    return false;
  }
  
  data.Goals.forEach (function (g) {
    nodes.push ({ id: g.Identifier, value: g });
  });
  
  data.Obstacles.forEach (function (o) {
    nodes.push ({ id: o.Identifier, value: o });
  });
 
  data.Agents.forEach (function (a) {
    nodes.push ({ id: a.Identifier, value: a });
  });
  
  data.DomainProperties.forEach (function (o) {
    nodes.push ({ id: o.Identifier, value: o });
  });
  
  data.DomainHypotheses.forEach (function (o) {
    nodes.push ({ id: o.Identifier, value: o });
  });
 
  data.GoalExceptions.forEach (function (o) {
    if (containsNode (o.AnchorGoalIdentifier) & containsNode (o.ResolvingGoalIdentifier)) {
      o.__kind = "exceptionEdge";
      edges.push ({ v: o.AnchorGoalIdentifier, u: o.ResolvingGoalIdentifier, value: o });
    }
  });
 
  data.Obstructions.forEach (function (o) {
    if (containsNode (o.ObstacleIdentifier) & containsNode (o.ObstructedGoalIdentifier)) {
      o.__kind = "obstructionEdge";
      edges.push ({ v: o.ObstructedGoalIdentifier, u: o.ObstacleIdentifier, value: o });
    }
  });
 
  data.Resolutions.forEach (function (o) {
    if (containsNode (o.ObstacleIdentifier) & containsNode (o.ResolvingGoalIdentifier)) {
      o.__kind = "resolutionEdge";
      edges.push ({ v: o.ObstacleIdentifier, u: o.ResolvingGoalIdentifier, value: o });
    }
  });

  data.Assignments.forEach (function (r) {
    nodes.push ({ id: r.Identifier, value: r });
    if (containsNode (r.GoalIdentifier)) {
      edges.push ({ v: r.GoalIdentifier, u: r.Identifier, value: { "__kind": "assignmentEdge" } });
    }
  
    r.AgentIdentifiers.forEach (function (aIdentifier) {
      if (containsNode (aIdentifier)) {
        edges.push ({ v: r.Identifier, u: aIdentifier, value: { "__kind": "agentEdge" } });
      }
    });
  });
  
  data.Refinements.forEach (function (r) {
    nodes.push ({ id: r.Identifier, value: r });
    if (containsNode (r.ParentGoalIdentifier)) {
      edges.push ({ v: r.ParentGoalIdentifier, u: r.Identifier, value: { "__kind": "refinementEdge" } });
    }
  
    r.SubGoalIdentifiers.forEach (function (sgIdentifier) {
      if (containsNode (sgIdentifier)) {
        edges.push ({ v: r.Identifier, u: sgIdentifier, value: { "__kind": "subgoalEdge" } });
      }
    });
  
    r.DomainPropertyIdentifiers.forEach (function (sgIdentifier) {
      if (containsNode (sgIdentifier)) {
        edges.push ({ v: r.Identifier, u: sgIdentifier, value: { "__kind": "subgoalEdge" } });
      }
    });
  
    r.DomainHypothesisIdentifiers.forEach (function (sgIdentifier) {
      if (containsNode (sgIdentifier)) {
        edges.push ({ v: r.Identifier, u: sgIdentifier, value: { "__kind": "subgoalEdge" } });
      }
    });
  });
  
  data.ObstacleRefinements.forEach (function (r) {
    nodes.push ({ id: r.Identifier, value: r });
    if (containsNode (r.ParentObstacleIdentifier)) {
      edges.push ({ v: r.ParentObstacleIdentifier, u: r.Identifier, value: { "__kind": "refinementEdge" } });
    }
  
    r.SubobstacleIdentifiers.forEach (function (sgIdentifier) {
      if (containsNode (sgIdentifier)) {
        edges.push ({ v: r.Identifier, u: sgIdentifier, value: { "__kind": "subObstacleEdge" } });
      }
    });
  });
  
  var g = dagreD3.json.decode(nodes, edges);
  
  var layout = dagreD3.layout()
                      .nodeSep(20)
                      .rankDir("BT");
  
  var renderer = new dagreD3.Renderer();

  /* Draw the arrow heads */
  var oldPostRender = renderer.postRender();
  renderer.postRender(function(graph,root){
    oldPostRender(graph, root);
    if (graph.isDirected() && root.select('#arrowhead-refinement').empty()) {      
        root.append('svg:defs')
            .append('svg:marker')
              .attr('id', 'arrowhead-refinement')
              .attr('viewBox', '0 0 10 8')
              .attr('refX', 10)
              .attr('refY', 4)
              // .attr('markerUnits', 'strokewidth')
              .attr('markerWidth', 10)
              .attr('markerHeight', 8)
              .attr('orient', 'auto')
              .attr('style', 'fill: white; stroke: black; stroke-width: 1')
              .append('svg:path')
                .attr('d', 'M 1 1 L 9 4 L 1 7 z');
    }
    
    if (graph.isDirected() && root.select('#arrowhead-assignment').empty()) {      
        root.append('svg:defs')
            .append('svg:marker')
              .attr('id', 'arrowhead-assignment')
              .attr('viewBox', '0 0 10 8')
              .attr('refX', 10)
              .attr('refY', 4)
              // .attr('markerUnits', 'strokewidth')
              .attr('markerWidth', 10)
              .attr('markerHeight', 8)
              .attr('orient', 'auto')
              .attr('style', 'fill: black; stroke: black; stroke-width: 1')
              .append('svg:path')
                .attr('d', 'M 1 1 L 9 4 L 1 7 z');
    }
    
    if (graph.isDirected() && root.select('#arrowhead-obstruction').empty()) {      
       var g = root.append('svg:defs')
            .append('svg:marker')
              .attr('id', 'arrowhead-obstruction')
              .attr('viewBox', '0 0 15 8')
              .attr('refX', 15)
              .attr('refY', 4)
              // .attr('markerUnits', 'strokewidth')
              .attr('markerWidth', 15)
              .attr('markerHeight', 8)
              .attr('orient', 'auto')
              .attr('style', 'fill: black; stroke: black; stroke-width: 1')
              .append('svg:g');
    
        g.append('svg:path')
                .attr('d', 'M 6 1 L 15 4 L 6 7 z');
                
        g.append('svg:path')
                .attr('d', 'M 1 1 L 1 7 z')
    }
    
    if (graph.isDirected() && root.select('#arrowhead-exception').empty()) {      
       var g = root.append('svg:defs')
            .append('svg:marker')
              .attr('id', 'arrowhead-exception')
              .attr('viewBox', '0 0 15 8')
              .attr('refX', 15)
              .attr('refY', 4)
              // .attr('markerUnits', 'strokewidth')
              .attr('markerWidth', 15)
              .attr('markerHeight', 8)
              .attr('orient', 'auto')
              .attr('style', 'fill: black; stroke: black; stroke-width: 1')
              .append('svg:g');
    
        g.append('svg:path')
                .attr('d', 'M 6 1 L 15 4 L 6 7 z');
                
        g.append('svg:path')
                .attr('d', 'M 1 1 L 1 7 z')
    }
    
    if (graph.isDirected() && root.select('#arrowhead-resolution').empty()) {      
       var g = root.append('svg:defs')
            .append('svg:marker')
              .attr('id', 'arrowhead-resolution')
              .attr('viewBox', '0 0 15 8')
              .attr('refX', 15)
              .attr('refY', 4)
              // .attr('markerUnits', 'strokewidth')
              .attr('markerWidth', 15)
              .attr('markerHeight', 8)
              .attr('orient', 'auto')
              .attr('style', 'fill: black; stroke: black; stroke-width: 1')
              .append('svg:g');
    
        g.append('svg:path')
                .attr('d', 'M 6 1 L 15 4 L 6 7 z');
                
        g.append('svg:path')
                .attr('d', 'M 1 1 L 1 7 z')
    }
  });

  /* Change the drawing of edge paths */
  var oldDrawEdgePaths = renderer.drawEdgePaths();
  renderer.drawEdgePaths(function(graph, root) {
    var svgEdges = oldDrawEdgePaths(graph, root);
    svgEdges.each(function(u) { 
        var edgeclass = graph.edge(u).__kind;
        if (edgeclass == "refinementEdge")
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'url(#arrowhead-refinement)')
        else if (edgeclass == "obstructionEdge")
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'url(#arrowhead-obstruction)')
        else if (edgeclass == "exceptionEdge") {
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'url(#arrowhead-exceptionEdge)');
        } else if (edgeclass == "resolutionEdge")
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'url(#arrowhead-resolution)')
        else if (edgeclass == "assignmentEdge")
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'url(#arrowhead-assignment)')
        else
          d3.select(this).select("path")
            .attr("class", graph.edge(u).edgeclass)
            .attr("marker-end", 'none')
    });
    return svgEdges;
  });

/***/

var addForeignObjectLabel = function (label, root) {
  var fo = root
    .append('foreignObject')
      .attr('width', '100000');

  var w, h;
  fo
    .append('xhtml:div')
      .style('float', 'left')
      // TODO find a better way to get dimensions for foreignObjects...
      .html(function() { return label; })
      .each(function() {
        w = this.clientWidth;
        h = this.clientHeight;
      });

  fo
    .attr('width', w)
    .attr('height', h);
}

var addTextLabel = function (label, root, labelCols, labelCut) {
  if (labelCut === undefined) labelCut = 'false';
  labelCut = (labelCut.toString().toLowerCase() === 'true');

  var node = root
    .append('text')
    .attr('text-anchor', 'left');

  label = label.replace(/\\n/g, '\n');

  var arr = labelCols ? wordwrap(label, labelCols, labelCut) : label;
  arr = arr.split('\n');
  for (var i = 0; i < arr.length; i++) {
    node
      .append('tspan')
        .attr('dy', '1em')
        .attr('x', '1')
        .text(arr[i]);
  }
}

var drawGoal = function (node, root, marginX, marginY) {
  var label = node.name;
  var poly = root.append('polygon');
  var labelSvg = root.append('g');
  
  var friendlyName = node.Name == null ? node.Identifier : node.Name;

  addForeignObjectLabel('<a href="goals.html#goal-'+node.Identifier+'" style="max-width: 250px; display: block;">'+friendlyName+'</a>', labelSvg);
  marginX = marginY = 5;

  var bbox = root.node().getBBox();

  labelSvg.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')');

  var skew = 5;
  var bottom = - marginX;
  var left = - marginX;
  var top = bbox.height + marginX;
  var right = bbox.width + marginX;
  var points = [{y: bottom, x: left}, 
                {y: bottom, x: right + skew}, 
                {y: top,    x: right}, 
                {y: top,    x: left - skew}]

  poly.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')')
    .attr ("points",function(d) { 
        return points.map(function(p) {
            return [p.x,p.y].join(",");
        }).join(" ")})
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', '#D8EBFE');
}

var drawObstacle = function (node, root) {
  var label = node.name;
  var poly = root.append('polygon');
  var labelSvg = root.append('g');
  
  var friendlyName = node.Name == null ? node.Identifier : node.Name;

  addForeignObjectLabel('<a href="obstacles.html#obstacle-'+node.Identifier+'" style="max-width: 250px; display: block;">'+friendlyName+'</a>', labelSvg);
  var marginX = 5;
  var marginY = 5;

  var bbox = root.node().getBBox();

  labelSvg.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')');

  var skew = -5;
  var bottom = - marginX;
  var left = - marginX;
  var top = bbox.height + marginX;
  var right = bbox.width + marginX;
  var points = [{y: bottom, x: left}, 
                {y: bottom, x: right + skew}, 
                {y: top,    x: right}, 
                {y: top,    x: left - skew}]

  poly.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')')
    .attr ("points",function(d) { 
        return points.map(function(p) {
            return [p.x,p.y].join(",");
        }).join(" ")})
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', '#FFA6AD');
}


var drawDomProp = function (node, root, marginX, marginY) {
  var label = node.name;
  var poly = root.append('polygon');
  var labelSvg = root.append('g');
  
  var friendlyName = node.Name == null ? node.Identifier : node.Name;

  addForeignObjectLabel('<a href="domprops.html#domprop-'+node.Identifier+'" style="max-width: 250px; display: block;">'+friendlyName+'</a>', labelSvg);
  marginX = marginY = 5;

  var bbox = root.node().getBBox();

  labelSvg.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')');

  var skew = 5;
  var bottom = - marginX;
  var left = - marginX;
  var top = bbox.height + marginX;
  var right = bbox.width + marginX;
  var points = [{y: bottom, x: left + skew}, 
                {y: bottom, x: right}, 
                {y: top,    x: right + skew}, 
                {y: top,    x: left}]

  poly.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')')
    .attr ("points",function(d) { 
        return points.map(function(p) {
            return [p.x,p.y].join(",");
        }).join(" ")})
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', '#E7FEC9');
}

var drawAgent = function (node, root) {
  var label = node.name;
  var poly = root.append('polygon');
  var labelSvg = root.append('g');
  
  var friendlyName = node.Name == null ? node.Identifier : node.Name;

  addForeignObjectLabel('<a href="agents.html#agent-'+node.Identifier+'" style="max-width: 250px; display: block;">'+friendlyName+'</a>', labelSvg);
  var marginX = 10;
  var marginY = 5;

  var bbox = root.node().getBBox();

  labelSvg.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')');

  var skew = 5;
  var bottom = - marginY;
  var left = - marginX;
  var top = bbox.height + marginY;
  var right = bbox.width + marginX;
  var points = [{y: bottom,                        x: left + skew}, 
                {y: bottom,                        x: right - skew}, 
                {y: Math.abs(bottom-top)/2+bottom, x: right}, 
                {y: top,                           x: right - skew}, 
                {y: top,                           x: left + skew}, 
                {y: Math.abs(bottom-top)/2+bottom, x: left}];

  poly.attr('transform',
             'translate(' + (-bbox.width / 2) + ',' + (-bbox.height / 2) + ')')
    .attr ("points",function(d) { 
        return points.map(function(p) {
            return [p.x,p.y].join(",");
        }).join(" ")})
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', '#DEBBFC');
}

var drawRefinementNode = function (node, root) {
  var rect = root.append('circle');
  var labelSvg = root.append('g');
  
  rect
    .attr('cx', 0)
    .attr('cy', 0)
    .attr('r', 5)
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', 'white');
}

var drawAssignmentNode = function (node, root) {
  var rect = root.append('circle');
  var labelSvg = root.append('g');
  
  rect
    .attr('cx', 0)
    .attr('cy', 0)
    .attr('r', 5)
    .attr('stroke', 'black')
    .attr('stroke-width', 1.5)
    .attr('fill', 'white');
}

renderer.drawNodes(function(g, root) {
  var nodes = g.nodes(); /* TODO? .filter(function(u) { return !isComposite(g, u); }); */

  var svgNodes = root
    .selectAll('g.node')
    .classed('enter', false)
    .data(nodes, function(u) { return u; });

  svgNodes.selectAll('*').remove();

  svgNodes
    .enter()
      .append('g')
        .style('opacity', 0)
        .attr('class', 'node enter');

  svgNodes.each(function(u) { 
    var node = g.node(u);
    if (node.__type == "Goal:#KAOSTools.MetaModel") {
      drawGoal(node, d3.select(this), 10, 10);
    } else if (node.__type == "GoalRefinement:#KAOSTools.MetaModel") {
      drawRefinementNode(node, d3.select(this));
    } else if (node.__type == "ObstacleRefinement:#KAOSTools.MetaModel") {
      drawRefinementNode(node, d3.select(this));
    } else if (node.__type == "GoalAgentAssignment:#KAOSTools.MetaModel") {
      drawAssignmentNode(node, d3.select(this));
    } else if (node.__type == "Obstacle:#KAOSTools.MetaModel") {
      drawObstacle(node, d3.select(this));
    } else if (node.__type == "Agent:#KAOSTools.MetaModel") {
      drawAgent(node, d3.select(this));
    } else if (node.__type == "DomainProperty:#KAOSTools.MetaModel") {
      drawDomProp(node, d3.select(this));
    }
    });

  this._transition(svgNodes.exit())
      .style('opacity', 0)
      .remove();

  return svgNodes;
});
  
  
/****/  

  
  var layout = renderer.layout(layout)
                       .edgeInterpolate('basis')
                       .run(g, d3.select(selector)
                                 .append("svg")
                                 .append("g"));
  
  d3.select(selector).select("svg")
    .attr("width", layout.graph().width)
    .attr("height", layout.graph().height);
    
}