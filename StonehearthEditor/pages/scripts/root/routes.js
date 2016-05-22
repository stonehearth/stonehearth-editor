console.log("foo");

Ember.Route.extend({
    model() {
        return ['Marie Curie', 'Mae Jemison', 'Albert Hofmann'];
    }
});

console.log("bar");