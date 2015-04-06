app.directive('userGravatar', [ 'md5', function (md5) {
	return {
		restrict: 'E',
		replace: true,
		link: function (scope, elem, attrs) {
			scope.gid = md5.createHash(attrs.email);
			scope.size = attrs.size | 80;
		},
		template: '<img src="https://secure.gravatar.com/avatar/{{gid}}?s={{size}}" alt="Gravatar profile image" />',
		scope: {
			email: '@',
			size: '@'
		}
	}
}])