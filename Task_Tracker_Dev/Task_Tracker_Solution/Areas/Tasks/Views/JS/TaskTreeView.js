function getTree() {
    // Some logic to retrieve, or generate tree structure
    var myTree = JSON.parse(taskTreeJson);
    return myTree;
}

$(function () {
    $('#tasktree').bind('mousewheel', function (e) {
        $(this).scrollTop($(this).scrollTop() - e.originalEvent.wheelDeltaY);
        return false;
    });

    var $searchableTree = $('#tasktree').treeview({
        data: getTree(),
        levels: 1
    });

    var search = function (e) {
        var input, filter, div, ul, li, a, i;
        var pattern = $('#input-search').val();
        var options = {
            ignoreCase: true,
            exactMatch: false,
            revealResults: true,
        };
        $searchableTree.treeview('search', [pattern, options]);

        //Scrolling to the element that is searched
        input = document.getElementById("input-search");
        filter = input.value.toUpperCase();
        div = document.getElementById("tree");
        li = div.getElementsByTagName("li");
        for (i = 0; i < li.length; i++) {
            a = li[i];
            if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {
                li[i].scrollIntoView(false)[0];

            }
        }
    }
    $('#btn-search').on('click', search);

    $('#btn-clear-search').on('click', function (e) {
        $searchableTree.treeview('clearSearch');
        $('#input-search').val('');
        $('#search-output').html('');
    });
});

//var myTree = [{
//    text: "Requirement Gathering",
//    nodes: [{
//        text: "Tree View Doc"
//    }]
//}, {
//    text: "Designing",
//    nodes: [{
//        text: "Design the layout for Tree View"

//    }, {
//        text: "Linking View"
//    }]
//}, {
//    text: "Development",
//    state: {
//        expanded: true,
//        selected: true
//    },
//    nodes: [
//           {
//               text: "Show the task in a tree View",
//               href: 'https://jmfl.com'
//           },
//         {
//             text: "Link the task to its parent Task",
//             href: 'https://jmfl.com'

//         }, ]
//}, {
//    text: "Deployment"
//}, {
//    text: "Testing",
//    nodes: [{
//        text: "Unit testing"
//    }, {
//        text: "Final Testing"
//    }]
//}];