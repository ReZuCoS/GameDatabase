create table languages(
    key varchar not null,
    name varchar not null,

    primary key(key)
);

insert into languages values ('EN', 'English');

create table users(
    login varchar not null,
    password varchar not null,
    salt varchar not null,
    totp_key varchar null,
	totp_recoveries varchar[] null,
    profile_image varchar null,
    language_key varchar not null default 'EN',

    primary key(login),
    foreign key(language_key) references languages(key) on delete set default
);

create table game_statuses(
    id int not null generated always as identity,
    name varchar not null,
    language_key varchar not null default 'EN',

   primary key(id),
   foreign key(language_key) references languages(key) on delete cascade
);

create table games(
    id int not null generated always as identity,
    image_vertical varchar not null,
    image_horizontal varchar not null,
    avg_playtime_in_hours smallint not null,
    release_date date not null,

    primary key(id)
);

create table game_translations(
    game_id int not null,
    title varchar not null,
    description varchar not null,
    language_key varchar not null default 'EN',

    primary key(game_id),
    foreign key(game_id) references games(id) on delete cascade,
    foreign key(language_key) references languages(key) on delete cascade
);

create table genres(
    id int not null generated always as identity,
    name varchar not null,

    primary key(id)
);

create table game_genres(
    game_id int not null,
    genre_id int not null,

    primary key(game_id, genre_id),
    foreign key(game_id) references games(id) on delete cascade,
    foreign key(genre_id) references genres(id) on delete cascade
);

create table tags(
    id int not null generated always as identity,
    name varchar not null,

    primary key(id)
);

create table game_tags(
    game_id int not null,
    tag_id int not null,

    primary key(game_id, tag_id),
    foreign key(tag_id) references tags(id) on delete cascade,
    foreign key(game_id) references games(id) on delete cascade
);

create table user_games(
    game_id int not null,
    user_login varchar not null,
    status int not null,
    custom_image_vertical varchar null,
    custom_image_horizontal varchar null,
    user_rate smallint null,

    primary key(game_id, user_login),
    foreign key(game_id) references games(id) on delete cascade,
    foreign key(user_login) references users(login) on delete cascade,
    foreign key(status) references game_statuses(id) on delete cascade
);
