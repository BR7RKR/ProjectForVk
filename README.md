
# Web Api For VK

Проект для вконтакте. Этот сервис умеет добавлять новые состояния и группы для пользователей, добавлять новых пользователей, блокировать пользователей, получать пользователей через пагинацию, получать одного пользователя по id.




## Documentation


Ход работы:

Сначала должен быть вручную создан админ в базе данных. Далее админ должен создать нужные роли и группы. С сервера будут приходить JSON пользователей с id, login, password, created_date,  user_group_id, user_state_id. Подразумевается, что пользователь будет приходить с состоянием blocked по умолчанию. Далее пользователь будет добавляться в базу данных, а через 5 секунд будет проводиться его активация. Для получения одного пользователя и блокировки одного пользователя нужно будет передать его id. Для получения нескольких пользователей нужно будет отправить запрос с pageNumber и pageSize. База данных PostgreSQL.

Настройка бд:

ConnectionString находится в appsettings.Development.json. Туда нужно вписать строку соединения для своей бд.  
Используется подход code first, но можно использовать подход Database first.

Примеры JSON запросов:

Добавление состояния {
  "id": 0,
  "code": "Blocked",
  "description": "string"
}

Добавление группы {
  "id": 0,
  "code": "User",
  "description": "string"
}

Добавление пользователя {
  "id": 0,
  "login": "string",
  "password": "string",
  "created_date": "2023-05-08",
  "user_group_id": 0,
  "user_state_id": 0
}

## Comments

Пароль: скорее всего пароль будет хэшироваться на стороне сайта, в теории можно добавить хэширование и в бэкенд.


Активация пользователя: в реальной ситуации я бы завел отдельный статус, который бы показывал неверифицированных пользователей. При регистрации на сайте пользователю бы выставлялся такой статус. Отдельный метод бы добавлял пользователя в бд, а метод регистрации бы активировался от другого запроса на сервер. Также я бы написал отдельный сервис, который бы раз в n секунд удалял пользователей, которые не прошли верификацию.


Тесты: тестирование данного сервиса юнит тестами, как по мне, бессмысленно. Так как вся логика приложения основана только на работе с базой данных. В реальной ситуации я бы прибегнул к тестам другого типа.
## Authors

- [@BR7RKR](https://github.com/BR7RKR)

