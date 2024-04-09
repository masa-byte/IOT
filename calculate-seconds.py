from datetime import datetime
import pytz


def get_seconds_since_epoch(date_string, time_zone):
    date_obj = datetime.strptime(date_string, "%Y-%m-%d %H:%M:%S")
    date_obj2 = datetime.strptime("1970-01-01 00:00:00", "%Y-%m-%d %H:%M:%S")

    # Set the time zone
    local_tz = pytz.timezone(time_zone)

    # Calculate the number of seconds since the epoch
    seconds_since_epoch = int((date_obj - date_obj2).total_seconds())

    # add the timezone offset
    seconds_since_epoch += int(local_tz.utcoffset(date_obj).total_seconds())

    return seconds_since_epoch


date_string = "2021-06-18 23:50:0"
time_zone = "GMT"

seconds_since_epoch = get_seconds_since_epoch(date_string, time_zone)
print(seconds_since_epoch)
