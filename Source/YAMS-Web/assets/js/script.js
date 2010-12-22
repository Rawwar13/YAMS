/* Author: Richard Benson
 * YAMS Javascripts
 */

function init() {
  $.ajax({
    url: "/api/",
    type: "POST",
    data: "list",
    dataType: "json",
    success: function(data) {
      $.each(data["servers"], function(i.item){
        $('main').add('div')
                 .attr('id', "server_" + item)
                 .html("This is server " + item);
      })
    }
  })
}

function getRows(start, rows, serverid, level) {
  $.ajax({
    url: "/api/",
    type: "POST",
    data: "action=log&start=" + start + "&rows=" + rows + "&serverid=" + serverid + "&level=" + level,
    dataType: "json",
    success: function(data) {
      $.each(data["Table"], function(i,item) {
        $('#log_' + level).html($('#log_' + level).html() + "<br />" + item["LogMessage"]);
      });
    }
  });
  return false;
};


$(document).ready(init);






















