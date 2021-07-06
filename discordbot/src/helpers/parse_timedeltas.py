import re
from datetime import timedelta

TIMEDELTA_REGEX = (r'((?P<days>\d+)d)?'
                   r'((?P<hours>\d+)h)?'
                   r'((?P<minutes>\d+)m)?')
TIMEDELTA_PATTERN = re.compile(TIMEDELTA_REGEX, re.IGNORECASE)


def parse_delta(delta):
    """ Parses a human readable timedelta (3d5h19m) into a datetime.timedelta.
    Delta includes:
    * Xd days
    * Xh hours
    * Xm minutes
    Values can be negative following timedelta's rules. Eg: -5h-30m
    """
    match = TIMEDELTA_PATTERN.match(delta)
    if match:
        parts = {k: int(v) for k, v in match.groupdict().items() if v}
        delta = timedelta(**parts)
        print(delta)
        if delta == timedelta(0):
            return None
        else:
            return delta
