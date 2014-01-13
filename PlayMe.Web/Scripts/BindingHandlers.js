ko.bindingHandlers.truncated = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor(), allBindings = allBindingsAccessor();

        // Next, whether or not the supplied model property is observable, get its current value
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        var truncatedLength = allBindings.truncatedLength || 60;

        $(element).text(valueUnwrapped.truncate(truncatedLength));
        $(element).attr("title",valueUnwrapped);
    }
};

ko.bindingHandlers.shortTime = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor();
        // Next, whether or not the supplied model property is observable, get its current value
        var unwrappedDateAndTime = Date.create(ko.utils.unwrapObservable(value));
        
        $(element).text(unwrappedDateAndTime.format("{mm}:{ss}"));
    }
};

ko.bindingHandlers.shortDateTime = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor();
        // Next, whether or not the supplied model property is observable, get its current value
        var unwrappedDateAndTime = Date.create(ko.utils.unwrapObservable(value));

        $(element).text(unwrappedDateAndTime.format("{yyyy}-{MM}-{dd} {HH}:{mm}:{ss}"));
    }
};

ko.bindingHandlers.formattedDateTime = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor();
        // Next, whether or not the supplied model property is observable, get its current value
        var unwrappedDateAndTime = Date.create(ko.utils.unwrapObservable(value.source));

        $(element).text(unwrappedDateAndTime.format(value.format || "full"));
    }
};

ko.bindingHandlers.timeAndDate = {
    update: function(element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor();
        // Next, whether or not the supplied model property is observable, get its current value
        var unwrappedDateAndTime = Date.create(ko.utils.unwrapObservable(value));
        $(element).text(unwrappedDateAndTime.format("{HH}:{mm} on {dd} {mon} {yyyy}"));
    }
};

ko.bindingHandlers.concatenated = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        // First get the latest data that we're bound to
        var value = valueAccessor(), allBindings = allBindingsAccessor();

        var separator = allBindings.concatSeparator || ", ";

        // Next, whether or not the supplied model property is observable, get its current value
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        $(element).text(valueUnwrapped.join(separator));
    }
};

ko.bindingHandlers.typeahead = {
    init: function (element, valueAccessor) {
        var binding = this;
        var elem = $(element);
        var value = valueAccessor();

        // Setup Bootstrap Typeahead for this element.
        elem.typeahead(
        {
            source: value.source,
            onselect: function (val) { value.target(val); }  
        });

        // Set the value of the target when the field is blurred.
        elem.blur(function () { value.target(elem.val()); });
        /*
        elem.on('change', function(event) {
            console.log("Changed: " + event.target.value);
            value.target(event.target.value);
            elem.closest("form").find("button").trigger("click");
        });*/
    },
    update: function (element, valueAccessor) {
        var elem = $(element);
        var value = valueAccessor();
        elem.val(value.target());
    }
    
};

ko.bindingHandlers.popover = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value.visible)) {
            var tempElement = $('<div/>').get(0);
            $('body').append(tempElement);
            ko.renderTemplate(value.template, bindingContext, {}, tempElement, 'replaceChildren');
            $(element).popover({
                content: tempElement.innerHTML,
                html: true,
                placement: 'top',
                title: value.title,
                trigger: 'hover'
            });
            //clean up node
            tempElement.parentNode.removeChild(tempElement);
        };
    }
};
