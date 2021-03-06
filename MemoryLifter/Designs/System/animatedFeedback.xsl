<?xml version='1.0' encoding='utf-16'?>
<!--  Animated feedback javascript  -->
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:CardObject="urn:cardobject"
	exclude-result-prefixes="CardObject">

	<xsl:template name="animatedFeedback">
		<xsl:text disable-output-escaping="yes"><![CDATA[
$(document).ready(function() {
	var isDragged = false;
	var feedbackBox = $("#feedback");
	feedbackBox.css('cursor', 'move');
	feedbackBox.draggable();
	feedbackBox.height(feedbackBox.height());	// otherwise the height is not properly kept
	feedbackBox.width(feedbackBox.width());	// otherwise the width is not properly kept			
	feedbackBox.bind('dragstart', function(event, ui) {
		isDragged = true;
	});
	var feedbackIconPath, delay;
	if ($("#feedbackImageCorrect").is('*')) {
		feedbackIconPath = stylePath + "feedbackCorrectSmall.gif";
		delay = 1000;
	} else {
		feedbackIconPath = stylePath + "feedbackWrongSmall.gif";
		delay = 2000;
	}
	var feedbackIcon = $(new Image(75, 75));
	feedbackIcon.load(function() {
		feedbackIcon.css('position', 'absolute');
		feedbackIcon.css('right', '0px');
		feedbackIcon.css('bottom', '0px');
		feedbackIcon.css('opacity', 0.7);
		feedbackIcon.hide();
		feedbackIcon.mouseover(function() {
			feedbackIcon.css('opacity', 0.9);
		});
		feedbackIcon.mouseout(function() {
			feedbackIcon.css('opacity', 0.7);
		});
		feedbackIcon.click(function() {
			feedbackIcon.fadeOut("fast", function() {
				feedbackBox.fadeIn("slow");
			});
		});
		$("#main").append(feedbackIcon);
	}).attr('src', feedbackIconPath);

	function closeBox() {
		feedbackBox.fadeOut("slow", function() {
			feedbackIcon.fadeIn("fast");
		});
	}
	
	var closeBtn = $('#closeBar button');
	closeBtn.click(closeBox);
	
	//automatically close the box
	$.timer(delay, function (timer) {
		if (!isDragged) closeBox();
		timer.stop();
	});
});
]]></xsl:text>
	</xsl:template>
</xsl:stylesheet>