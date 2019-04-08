# Generated by Django 2.1.2 on 2018-11-09 16:41

from django.conf import settings
from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    initial = True

    dependencies = [
        migrations.swappable_dependency(settings.AUTH_USER_MODEL),
    ]

    operations = [
        migrations.CreateModel(
            name='AuthGroup',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=80, unique=True)),
            ],
            options={
                'db_table': 'auth_group',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='AuthGroupPermissions',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
            ],
            options={
                'db_table': 'auth_group_permissions',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='AuthPermission',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=255)),
                ('codename', models.CharField(max_length=100)),
            ],
            options={
                'db_table': 'auth_permission',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='AuthUser',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('password', models.CharField(max_length=128)),
                ('last_login', models.DateTimeField(blank=True, null=True)),
                ('is_superuser', models.IntegerField()),
                ('username', models.CharField(max_length=150, unique=True)),
                ('first_name', models.CharField(max_length=30)),
                ('last_name', models.CharField(max_length=150)),
                ('email', models.CharField(max_length=254)),
                ('is_staff', models.IntegerField()),
                ('is_active', models.IntegerField()),
                ('date_joined', models.DateTimeField()),
            ],
            options={
                'db_table': 'auth_user',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='AuthUserGroups',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
            ],
            options={
                'db_table': 'auth_user_groups',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='AuthUserUserPermissions',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
            ],
            options={
                'db_table': 'auth_user_user_permissions',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='DjangoAdminLog',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('action_time', models.DateTimeField()),
                ('object_id', models.TextField(blank=True, null=True)),
                ('object_repr', models.CharField(max_length=200)),
                ('action_flag', models.PositiveSmallIntegerField()),
                ('change_message', models.TextField()),
            ],
            options={
                'db_table': 'django_admin_log',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='DjangoContentType',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('app_label', models.CharField(max_length=100)),
                ('model', models.CharField(max_length=100)),
            ],
            options={
                'db_table': 'django_content_type',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='DjangoMigrations',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('app', models.CharField(max_length=255)),
                ('name', models.CharField(max_length=255)),
                ('applied', models.DateTimeField()),
            ],
            options={
                'db_table': 'django_migrations',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='DjangoSession',
            fields=[
                ('session_key', models.CharField(max_length=40, primary_key=True, serialize=False)),
                ('session_data', models.TextField()),
                ('expire_date', models.DateTimeField()),
            ],
            options={
                'db_table': 'django_session',
                'managed': False,
            },
        ),
        migrations.CreateModel(
            name='Department',
            fields=[
                ('name', models.CharField(max_length=64, primary_key=True, serialize=False)),
            ],
            options={
                'db_table': 'department',
            },
        ),
        migrations.CreateModel(
            name='Device',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('buydate', models.DateField(db_column='buyDate')),
                ('serialnumber', models.CharField(db_column='serialNumber', max_length=64)),
            ],
            options={
                'db_table': 'device',
            },
        ),
        migrations.CreateModel(
            name='DevicePersonAdmin',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('device', models.ForeignKey(db_column='device', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Device')),
            ],
            options={
                'db_table': 'device_person_admin',
            },
        ),
        migrations.CreateModel(
            name='DevicePersonUsed',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('device', models.ForeignKey(db_column='device', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Device')),
            ],
            options={
                'db_table': 'device_person_used',
            },
        ),
        migrations.CreateModel(
            name='Devicetype',
            fields=[
                ('id', models.IntegerField(primary_key=True, serialize=False)),
                ('name', models.CharField(max_length=64)),
            ],
            options={
                'db_table': 'devicetype',
            },
        ),
        migrations.CreateModel(
            name='Manufacturer',
            fields=[
                ('name', models.CharField(max_length=64, primary_key=True, serialize=False)),
                ('address', models.CharField(max_length=64)),
                ('contact', models.CharField(max_length=64)),
            ],
            options={
                'db_table': 'manufacturer',
            },
        ),
        migrations.CreateModel(
            name='Person',
            fields=[
                ('pid', models.CharField(max_length=64, primary_key=True, serialize=False)),
                ('name', models.CharField(max_length=64)),
                ('surname', models.CharField(max_length=64)),
                ('mail', models.CharField(max_length=64)),
                ('admin', models.IntegerField(blank=True, null=True)),
            ],
            options={
                'db_table': 'person',
            },
        ),
        migrations.CreateModel(
            name='PersonDepartment',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('department', models.ForeignKey(db_column='department', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Department')),
                ('person', models.ForeignKey(db_column='person', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Person')),
            ],
            options={
                'db_table': 'person_department',
            },
        ),
        migrations.CreateModel(
            name='Repair',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('reported', models.DateField()),
                ('finished', models.DateField(blank=True)),
                ('administrated_by', models.ForeignKey(db_column='administrated_by', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Person')),
                ('device', models.ForeignKey(db_column='device', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Device')),
                ('reported_by', models.ForeignKey(db_column='reported_by', on_delete=django.db.models.deletion.DO_NOTHING, related_name='reported_by', to='myapp.Person')),
            ],
            options={
                'db_table': 'repair',
            },
        ),
        migrations.CreateModel(
            name='Room',
            fields=[
                ('room', models.CharField(max_length=64, primary_key=True, serialize=False)),
            ],
            options={
                'db_table': 'room',
            },
        ),
        migrations.AddField(
            model_name='person',
            name='room',
            field=models.ForeignKey(db_column='room', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Room'),
        ),
        migrations.AddField(
            model_name='person',
            name='user',
            field=models.OneToOneField(blank=True, null=True, on_delete=django.db.models.deletion.SET_NULL, to=settings.AUTH_USER_MODEL),
        ),
        migrations.AddField(
            model_name='devicepersonused',
            name='person',
            field=models.ForeignKey(db_column='person', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Person'),
        ),
        migrations.AddField(
            model_name='devicepersonadmin',
            name='person',
            field=models.ForeignKey(db_column='person', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Person'),
        ),
        migrations.AddField(
            model_name='device',
            name='dadmin',
            field=models.ManyToManyField(through='myapp.DevicePersonAdmin', to='myapp.Person'),
        ),
        migrations.AddField(
            model_name='device',
            name='department',
            field=models.ForeignKey(db_column='department', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Department'),
        ),
        migrations.AddField(
            model_name='device',
            name='dused',
            field=models.ManyToManyField(related_name='dused', through='myapp.DevicePersonUsed', to='myapp.Person'),
        ),
        migrations.AddField(
            model_name='device',
            name='manufacturer',
            field=models.ForeignKey(db_column='manufacturer', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Manufacturer'),
        ),
        migrations.AddField(
            model_name='device',
            name='room',
            field=models.ForeignKey(db_column='room', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Room'),
        ),
        migrations.AddField(
            model_name='device',
            name='type',
            field=models.ForeignKey(db_column='type', on_delete=django.db.models.deletion.DO_NOTHING, to='myapp.Devicetype'),
        ),
    ]
