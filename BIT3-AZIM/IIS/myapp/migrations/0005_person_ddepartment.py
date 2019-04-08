# Generated by Django 2.1.2 on 2018-11-21 11:37

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('myapp', '0004_remove_person_admin'),
    ]

    operations = [
        migrations.AddField(
            model_name='person',
            name='ddepartment',
            field=models.ManyToManyField(through='myapp.PersonDepartment', to='myapp.Department'),
        ),
    ]
