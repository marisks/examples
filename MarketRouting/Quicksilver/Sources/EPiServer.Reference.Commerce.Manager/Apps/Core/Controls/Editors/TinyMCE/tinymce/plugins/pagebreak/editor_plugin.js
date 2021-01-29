(function(){tinymce.create("tinymce.plugins.PageBreakPlugin",{init:function(b,d){var e='<img src="'+d+'/img/trans.gif" class="mcePageBreak mceItemNoResize" />',f="mcePageBreak",a=b.getParam("pagebreak_separator","<!-- pagebreak -->"),c;c=new RegExp(a.replace(/[\?\.\*\[\]\(\)\{\}\+\^\$\:]/g,function(g){return"\\"+g;}),"g");b.addCommand("mcePageBreak",function(){b.execCommand("mceInsertContent",0,e);});b.addButton("pagebreak",{title:"pagebreak.desc",cmd:f});b.onInit.add(function(){if(b.settings.content_css!==false){b.dom.loadCSS(d+"/css/content.css");}if(b.theme.onResolveName){b.theme.onResolveName.add(function(g,h){if(h.node.nodeName=="IMG"&&b.dom.hasClass(h.node,f)){h.name="pagebreak";}});}});b.onClick.add(function(g,h){h=h.target;if(h.nodeName==="IMG"&&g.dom.hasClass(h,f)){g.selection.select(h);}});b.onNodeChange.add(function(i,h,g){h.setActive("pagebreak",g.nodeName==="IMG"&&i.dom.hasClass(g,f));});b.onBeforeSetContent.add(function(g,h){h.content=h.content.replace(c,e);});b.onPostProcess.add(function(g,h){if(h.get){h.content=h.content.replace(/<img[^>]+>/g,function(i){if(i.indexOf('class="mcePageBreak')!==-1){i=a;}return i;});}});},getInfo:function(){return{longname:"PageBreak",author:"Moxiecode Systems AB",authorurl:"http://tinymce.moxiecode.com",infourl:"http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/pagebreak",version:tinymce.majorVersion+"."+tinymce.minorVersion};}});tinymce.PluginManager.add("pagebreak",tinymce.plugins.PageBreakPlugin);})();