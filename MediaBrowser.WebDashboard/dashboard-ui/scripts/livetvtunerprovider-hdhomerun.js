define(["jQuery"],function($){"use strict";function reload(page,providerId){page.querySelector(".txtDevicePath").value="",page.querySelector(".chkFavorite").checked=!1,providerId?ApiClient.getNamedConfiguration("livetv").then(function(config){var info=config.TunerHosts.filter(function(i){return i.Id==providerId})[0];page.querySelector(".txtDevicePath").value=info.Url||"",page.querySelector(".chkFavorite").checked=info.ImportFavoritesOnly,page.querySelector(".chkTranscode").checked=info.AllowHWTranscoding,page.querySelector(".chkEnabled").checked=info.IsEnabled}):page.querySelector(".chkEnabled").checked=!0}function submitForm(page){Dashboard.showLoadingMsg();var info={Type:"hdhomerun",Url:page.querySelector(".txtDevicePath").value,ImportFavoritesOnly:page.querySelector(".chkFavorite").checked,AllowHWTranscoding:page.querySelector(".chkTranscode").checked,IsEnabled:page.querySelector(".chkEnabled").checked,DataVersion:1},id=getParameterByName("id");id&&(info.Id=id),ApiClient.ajax({type:"POST",url:ApiClient.getUrl("LiveTv/TunerHosts"),data:JSON.stringify(info),contentType:"application/json"}).then(function(result){Dashboard.processServerConfigurationUpdateResult(),Dashboard.navigate("livetvstatus.html")},function(){Dashboard.alert({message:Globalize.translate("ErrorSavingTvProvider")})})}$(document).on("pageinit","#liveTvTunerProviderHdHomerunPage",function(){var page=this;$("form",page).on("submit",function(){return submitForm(page),!1})}).on("pageshow","#liveTvTunerProviderHdHomerunPage",function(){var providerId=getParameterByName("id"),page=this;reload(page,providerId)})});