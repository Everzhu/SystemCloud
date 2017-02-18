app.controller('DictBloodListController', function ($scope, $rootScope, $http, $location, $routeParams) {
    $scope.search = function () {
        $http.get('/api/Dict/Blood/List', {
            params: {
                "searchText": $rootScope.searchText
            }
        }).success(function (data) {
            $scope.BloodList = data
        })
    }

    var ids = "";
    $scope.check = function (val, chk) {
        if (!chk == true) {
            ids += val + ",";
        } else {
            ids = ids.replace(val + ",", "");
        }
    };

    $scope.delete = function () {
        $http.post('/api/Dict/Blood/Delete', JSON.stringify(ids)
            ).success(function (data) {
                if (data) {
                    $scope.search();
                }
                else {
                    alert('删除错误!');
                }
            });
    }

    $scope.add = function () {
        $location.path('/Dict/Blood/' + 0);
    }

    $scope.search();
})

app.controller('DictBloodEditController', function ($scope, $rootScope, $http, $location, $routeParams) {
    $http.get('/api/Dict/Blood/edit', {
        params: {
            "id": $routeParams.id,
        }
    }).success(function (data) {
        $scope.BloodEdit = data;
    })

    $scope.save = function () {
        if ($scope.myForm.$invalid) {
            alert('表单填写不完整');
            return false;
        }
        else {
            alert('OK');
            return false;
        }

        $http.post('/api/Dict/Blood/Edit', JSON.stringify($scope.BloodEdit)
            ).success(function (data) {
                if (data) {
                    if (data) {
                        $scope.cancel();
                    }
                    else {
                        alert('报错错误！');
                    }
                }
                else {
                    alert('删除错误!');
                }
            });
    }

    $scope.cancel = function () {
        $location.path("/Dict/Blood");
    }
})